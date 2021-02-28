using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper.Contrib.Extensions;
using MultiTableRepository.Fluent;
using MultiTableRepository.Attributes;

namespace MultiTableRepository.Parser
{
    public static class MultiTableParserV2
    {
        internal const string SEGMENT_SEPARATOR = "_";

        #region Simple memory cache

        private class CacheHeader
        {
            public Type EntityType { get; set;  }
            public string TablePrefix { get; set; }
            public string KeyColumn { get; set; }
            public IList<PropertyInfo> Segments { get; set; }
            public IList<string> WritableColumns { get; set; }
            public IList<string> AllColumns { get; set; }
            public PropertyInfo KeyProperty { get; set; }
            public IList<PropertyInfo> WritableProperties { get; set; }
            public IList<PropertyInfo> AllProperties { get; set; }

            public bool HasVariants { get; set; }
            public Dictionary<PropertyInfo, IEnumerable<string[]>> IgnoredSegments { get; set; }
            public Dictionary<PropertyInfo, IEnumerable<string[]>> ExclusiveSegments { get; set; }

            public Dictionary<string, CacheItem> Items { get; set; } = new Dictionary<string, CacheItem>();
        }

        private class CacheItem : ITableInfo
        {
            public CacheItem(CacheHeader parent)
            {
                Parent = parent;
            }

            public CacheHeader Parent { get; }

            public Type EntityType => Parent?.EntityType;
            public string TablePrefix => Parent?.TablePrefix;
            public string KeyColumn => Parent?.KeyColumn;
            public PropertyInfo KeyProperty => Parent?.KeyProperty;

            public string TableName { get; set; }
            public string TableSuffix { get; set; }
            public IEnumerable<string> AllColumns { get; set; }
            public IEnumerable<string> WritableColumns { get; set; }

            public IEnumerable<PropertyInfo> WritableProperties { get; set; }
            public IEnumerable<PropertyInfo> AllProperties { get; set; }

            public SqlParts SqlTemplates { get; set; }
        }

        private static readonly Dictionary<Type, CacheHeader> Cache = new Dictionary<Type, CacheHeader>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads information about a database table for the model class <typeparamref name="T"/> configured
        /// as multi-table, using the values in <paramref name="entity"/> to find the correct segments.
        /// </summary>
        /// <typeparam name="T">Model class configured with <see cref="MultiTableAttribute"/> and
        /// <see cref="SegmentAttribute"/></typeparam>.
        /// <param name="entity">Objec containing values to define the targe table segments.</param>
        /// <returns><see cref="ITableInfo"/> object containing information about the target table.</returns>
        public static ITableInfo GetTableInformation<T>(T entity)
        {
            if(entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (Cache.TryGetValue(typeof(T), out var parent))
            {
                var segements = GetSegmentsValues(parent.Segments, entity);
                var suffix = GetSuffix(segements);
                if (parent.Items.TryGetValue(suffix, out var item))
                {
                    return item;
                }
                else
                {
                    return CreateAndCacheItem(parent, segements);
                }
            }
            else
            {
                return ParsePropertiesAndAttributes(entity);
            }
        }

        /// <summary>
        /// Loads information about a database table for the model class <typeparamref name="T"/> configured
        /// as multi-table, using the passed comma-separated list of string as segment values.
        /// </summary>
        /// <typeparam name="T">Model class configured with <see cref="MultiTableAttribute"/> and
        /// <see cref="SegmentAttribute"/></typeparam>.
        /// <param name="segments">Strings used to define the targe table segments.</param>
        /// <returns><see cref="ITableInfo"/> object containing information about the target table.</returns>
        public static ITableInfo GetTableInformation<T>(params string[] segments)
        {
            if(segments == null || segments.Length == 0)
            {
                throw new ArgumentNullException(nameof(segments));
            }

            if (Cache.TryGetValue(typeof(T), out var parent))
            {
                var suffix = GetSuffix(segments);
                if (parent.Items.TryGetValue(suffix, out var item))
                {
                    return item;
                }
                else
                {
                    return CreateAndCacheItem(parent, segments);
                }
            }
            else
            {
                return ParsePropertiesAndAttributes<T>(default, segments);
            }
        }

        #endregion

        #region Parsing

        private static CacheItem ParsePropertiesAndAttributes<T>(T entity, string[] segments = null)
        {
            var type = typeof(T);

            var segProps = new SortedDictionary<int, PropertyInfo>();
            var dataProps = new List<PropertyInfo>();
            var allProps = new List<PropertyInfo>();
            var ignoreProps = new Dictionary<PropertyInfo, IEnumerable<string[]>>();
            var exclusiveProps = new Dictionary<PropertyInfo, IEnumerable<string[]>>();
            var keyProp = default(PropertyInfo);
            var hasVariants = false;


            var tablePrefix = type.GetCustomAttribute<MultiTableAttribute>()?.TableNamePrefix?.ToUpper();

            if (tablePrefix == null)
            {
                throw new Exception($"Table name not defined for {type.FullName}. Missing {nameof(MultiTableAttribute)}(\"TABLE_PREFIX\").");
            }

            var propertyInfos = type.GetProperties();

            foreach (var info in propertyInfos)
            {
                if (info.IsDefined(typeof(KeyAttribute), false))
                {
                    if (keyProp != null)
                    {
                        throw new Exception($"Multi-table entity {type.FullName} can't have more than one {nameof(KeyAttribute)}."); //TODO: Support for multi-keys?
                    }

                    keyProp = info;
                    allProps.Add(info);
                    continue;
                }

                var segmentIndex = info.GetCustomAttribute<SegmentAttribute>(false)?.Index;
                if (segmentIndex.HasValue)
                {
                    try
                    {
                        segProps.Add(segmentIndex.Value, info);
                        allProps.Add(info);
                        continue;
                    }
                    catch (ArgumentException ex)
                    {
                        throw new ArgumentException($"Entity {type.FullName} has more than one property marked as {nameof(SegmentAttribute)} with index {segmentIndex}.", ex);
                    }
                }

                // Variable columns (Ignored and exclusive)
                var ignoredSegments = info.GetCustomAttributes<IgnoreForAttribute>(false).Select(a => a.Segments);
                var exclusiveSegments = info.GetCustomAttributes<ExclusiveForAttribute>(false).Select(a => a.Segments);

                if (ignoredSegments.Any())
                {
                    ignoreProps.Add(info, ignoredSegments);
                }

                if (exclusiveSegments.Any())
                {
                    exclusiveProps.Add(info, exclusiveSegments);
                }

                hasVariants = hasVariants || ignoreProps.Any() || exclusiveProps.Any();

                // Writable columns
                if (info.GetSetMethod() != null && !info.IsDefined(typeof(ComputedAttribute), false) && !info.IsDefined(typeof(SegmentAttribute), false))
                {
                    dataProps.Add(info);
                }

                allProps.Add(info); //TODO: remove variant columns
            }

            if (segProps.Count == 0)
            {
                throw new Exception($"Entity {type.FullName} is expected to be multi-table but has no {nameof(SegmentAttribute)}s set.");
            }

            if (dataProps.Count == 0)
            {
                throw new Exception($"Entity {type.FullName} has no writable columns. Remember columns with {nameof(KeyAttribute)} (identity), {nameof(ComputedAttribute)} or {nameof(SegmentAttribute)} are read-only.");
            }

            if (segments == null)
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }
                segments = GetSegmentsValues(segProps.Values, entity);
            }

            var header = new CacheHeader
            {
                EntityType = type,
                TablePrefix = tablePrefix,
                KeyProperty = keyProp,
                Segments = segProps.Values.ToList(),
                HasVariants = hasVariants,
                AllProperties = allProps,
                WritableProperties = dataProps,
                KeyColumn = keyProp.Name,
                AllColumns = allProps.Select(p => p.Name).ToList(),
                WritableColumns = dataProps.Select(p => p.Name).ToList(),
                IgnoredSegments = ignoreProps,
                ExclusiveSegments = exclusiveProps,
            };

            var item = CreateAndCacheItem(header, segments);

            //Main entry
            Cache.Add(type, header);

            return item;
        }

        private static CacheItem CreateAndCacheItem(CacheHeader parent, string[] segments) // TODO: Move to CacheHeader class when decopling
        {
            if (segments == null || segments.Length != parent.Segments.Count) //TODO: Optional segments?
            {
                throw new ArgumentOutOfRangeException(nameof(segments), $"Multi-table model '{parent.EntityType.FullName}' requires {parent.Segments.Count} segment(s) but {segments.Length} was provided.");
            }

            var suffix = GetSuffix(segments);

            var item = new CacheItem(parent)
            {
                TableName = parent.TablePrefix + SEGMENT_SEPARATOR + suffix,
                TableSuffix = suffix,
                AllColumns = parent.AllColumns.ToList(),
                WritableColumns = parent.WritableColumns.ToList(),
                WritableProperties = parent.WritableProperties.ToList(),
                AllProperties = parent.AllProperties.ToList(),
                SqlTemplates = new SqlParts(),
            };

            BuildSqlTextTemplates(item);

            if (parent.HasVariants)
            {
                SetVariant(item, segments);
            }

            parent.Items.Add(suffix, item);

            return item;
        }


        private static void SetVariant(CacheItem item, string[] segments) // TODO: Move to CacheItem class when decopling
        {
            if (segments == null || !segments.Any())
            {
                throw new ArgumentNullException(nameof(segments));
            }

            var removed = new List<PropertyInfo>();
            foreach (var prop in item.AllProperties)
            {
                if (removed.Contains(prop))
                {
                    continue;
                }

                // If segments match, remove this property
                if (item.Parent.IgnoredSegments.TryGetValue(prop, out var ignoredSegments))
                {
                    var ignoreProp = false;

                    foreach (var seg in ignoredSegments)
                    {
                        var ignore = true;
                        for (int i = 0; i < segments.Length; i++)
                        {
                            if (seg.Length <= i)
                            {
                                break;
                            }

                            if (seg[i] == null)
                            {
                                continue;
                            }

                            ignore = ignore && (seg[i].Equals(segments[i], StringComparison.OrdinalIgnoreCase));
                        }

                        ignoreProp = ignoreProp || ignore;
                    }

                    if (ignoreProp)
                    {
                        removed.Add(prop);
                    }
                }

                // If segments match, keep this property
                if (item.Parent.ExclusiveSegments.TryGetValue(prop, out var exclusiveSegments))
                {
                    var keepProp = false;

                    foreach (var exSegs in exclusiveSegments)
                    {
                        var keep = true;

                        for (int i = 0; i < segments.Length; i++)
                        {
                            if (exSegs.Length <= i)
                            {
                                break;
                            }

                            if (exSegs[i] == null)
                            {
                                continue;
                            }

                            keep = keep && (exSegs[i].Equals(segments[i], StringComparison.OrdinalIgnoreCase));
                        }

                        keepProp = keepProp || keep;
                    }

                    if (!keepProp)
                    {
                        removed.Add(prop);
                    }
                }
            }

            // Copy property lists to break reference with parent lists
            var all = item.Parent.AllProperties.ToList();
            var wri = item.Parent.WritableProperties.ToList();

            foreach (var prop in removed)
            {
                all.Remove(prop);
                wri.Remove(prop);
            }

            if (all.Count != item.AllProperties.Count())
            {
                item.AllProperties = all;
                item.AllColumns = all.Select(p => p.Name).ToList();
                item.WritableProperties = wri;
                item.WritableColumns = wri.Select(p => p.Name).ToList();
            }
        }

        private static void BuildSqlTextTemplates(CacheItem item) // TODO: Move to CacheItem class when decopling
        {
            SqlParts parts = new SqlParts();

            //SELECT
            const string SEPARATOR = ", ";
            parts.SelectColumns = string.Join(SEPARATOR, item.AllColumns);

            //INSERT
            var dataCols = item.WritableColumns;
            parts.InsertColumnNames = $" ({string.Join(SEPARATOR, dataCols)})";
            parts.InsertColumnValues = $" VALUES (@{string.Join(", @", dataCols)}) ";

            //UPDATE
            var sb = new StringBuilder();
            foreach(var colName in item.WritableColumns)
            {
                if (sb.Length > 0) sb.Append(SEPARATOR).Append(' ');
                sb.Append(colName).Append(" = @").Append(colName);
            }
            parts.UpdateSetColumns = sb.ToString();

            item.SqlTemplates = parts;
        }

        private static string GetSuffix(params string[] segments)
        {
            return string.Join(SEGMENT_SEPARATOR, segments);
        }
        private static string[] GetSegmentsValues<T>(IEnumerable<PropertyInfo> segProps, T entity) // TODO: Move to CacheHeader class when decopling
        {
            return segProps.Select(p => p.GetValue(entity)?.ToString()).ToArray();
        }

        #endregion
    }
}

