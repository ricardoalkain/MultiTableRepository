using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper.Contrib.Extensions;
using MultiTableRepository.Attributes;
using MultiTableRepository.Parser.V1;

namespace MultiTableRepository.Parser.V1
{
    [Obsolete]
    public static class MultiTableParser
    {
        private class TableInfo : ITableInfoV1  //TODO: Join with CacheEntry
        {
            private readonly CacheEntry cached;

            public TableInfo(CacheEntry cacheEntry, string suffix)
            {
                TableSuffix = suffix;
                cached = cacheEntry;
            }

            public string TableSuffix { get; set; }

            public string TableName => $"{TablePrefix}_{TableSuffix}";

            public Type EntityType => cached.EntityType;

            public string TablePrefix => cached.TablePrefix;

            public PropertyInfo KeyProperty => cached.KeyProperty;

            public IReadOnlyList<PropertyInfo> Segments => cached.Segments;

            public bool HasVariants => cached.HasVariants;

            public IReadOnlyList<PropertyInfo> AllColumns => cached.AllColumns;

            public IReadOnlyList<PropertyInfo> DataColumns => cached.DataColumns;

            public string SqlSelectColumnsText => cached.SqlSelectColumnsText;

            public string SqlInsertColumnNames => cached.SqlInsertColumnNames;

            public string SqlInsertColumnValues => cached.SqlInsertColumnValues;

            public string SqlUpdateColumnsText => cached.SqlUpdateColumnsText;
        }

        private class CacheEntry
        {
            public Type EntityType { get; set; }

            public string TablePrefix { get; set; }

            public PropertyInfo KeyProperty { get; set; }

            public IReadOnlyList<PropertyInfo> Segments { get; set; }

            public IReadOnlyList<PropertyInfo> AllColumns { get; set; }

            public IReadOnlyList<PropertyInfo> DataColumns { get; set; }

            public string SqlSelectColumnsText { get; set; }

            public string SqlInsertColumnNames { get; set; }

            public string SqlInsertColumnValues { get; set; }

            public string SqlUpdateColumnsText { get; set; }

            public bool HasVariants { get; set; }

            public Dictionary<string, ITableInfoV1> Variants = new Dictionary<string, ITableInfoV1>(); //TODO: Create only if needed

        }

        private static Dictionary<Type, CacheEntry> Cache { get; } = new Dictionary<Type, CacheEntry>();

        #region Public Members

        /// <summary>
        /// Retrieve information about the tables where entities of this type are persisted
        /// and how the respective columns are configured.
        /// </summary>
        /// <param name="type">Type of entities associated to the table.</param>
        /// <param name="segments">List of strings used to separate data in different individual tables.</param>
        /// <returns><see cref="ITableInfoV1"/> object containing information about the tables and columns.</returns>
        public static ITableInfoV1 GetTableInfo(Type type, params string[] segments)
        {
            if (Cache.TryGetValue(type, out CacheEntry entry))
            {
                if (entry.HasVariants)
                {
                    var suffix = GetTableSuffix(segments);
                    if (entry.Variants.TryGetValue(suffix, out var variant))
                    {
                        return variant;
                    }
                }
                else
                {
                    return ParseTypeAndSegments(type, segments);
                }
            }

            return ParseTypeAndSegments(type, segments);
        }

        /// <summary>
        /// Retrieve information about the tables where entities of this type are persisted
        /// and how the respective columns are configured.
        /// </summary>
        /// <typeparam name="T">Type of entities associated to the table.</typeparam>
        /// <returns><see cref="ITableInfoV1"/> object containing information about the tables and columns.</returns>
        public static ITableInfoV1 GetTableInfo<T>()
        {
            return GetTableInfo(default(T));
        }

        /// <summary>
        /// Retrieve information about the tables where entities of this type are persisted
        /// and how the respective columns are configured.
        /// </summary>
        /// <typeparam name="T">Type of entities associated to the table.</typeparam>
        /// <param name="entity">Object containing segment properties to define which table it belongs to.</param>
        /// <returns><see cref="ITableInfoV1"/> object containing information about the tables and columns.</returns>
        public static ITableInfoV1 GetTableInfo<T>(T entity)
        {
            var type = typeof(T);

            if (Cache.TryGetValue(type, out CacheEntry entry))
            {
                var suffix = GetTableSuffix(entity, entry);

                if (!string.IsNullOrEmpty(suffix) && entry.HasVariants)
                {
                    if (entry.Variants.TryGetValue(suffix, out var variant))
                    {
                        return variant;
                    }
                    else
                    {
                        return ParseTypeAndSegments(entity);
                    }
                }
                else
                {
                    return new TableInfo(entry, suffix); //TODO: Cache non-variant suffixed tables
                }
            }

            ParsePropertiesAndAttributes(type);
            return GetTableInfo(entity);
        }

        /// <summary>
        /// Returns an array of strings containing values used to create a table suffix,
        /// according to <see cref="SegmentAttribute"/> in the provided entity.
        /// </summary>
        /// <typeparam name="T">Type of entities associated to the table.</typeparam>
        /// <param name="entity">Object containing segment properties to define which table it belongs to.</param>
        /// <returns>Array of strings.</returns>
        public static string[] GetSegments<T>(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(GetSegments)}: Entity can't be null.");
            }

            if (!Cache.TryGetValue(typeof(T), out var entry))
            {
                entry = ParsePropertiesAndAttributes(typeof(T));
            }

            return GetSegments(entity, entry);
        }

        #endregion





        private static CacheEntry ParsePropertiesAndAttributes(Type type)
        {
            var segProps = new SortedDictionary<int, PropertyInfo>();
            var dataProps = new List<PropertyInfo>();
            var allProps = new List<PropertyInfo>();
            var keyProp = default(PropertyInfo);
            var hasVariants = false;

            //?? Use TableAttibute or use custom MultiTableAttribute to avoid clash?
            var tablePrefix = type.GetCustomAttribute<MultiTableAttribute>()?.TableNamePrefix?.ToUpper();

            if (tablePrefix == null)
            {
                throw new Exception($"Table name not defined for {type.FullName}. Missing {nameof(MultiTableAttribute)}.");
            }

            var propertyInfos = type.GetProperties();

            foreach (var info in propertyInfos)
            {
                if (info.IsDefined(typeof(KeyAttribute), false))
                {
                    if (keyProp != null)
                    {
                        throw new Exception($"Multi-table entity {type.FullName} can't have more than one {nameof(KeyAttribute)}.");
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

                if (!hasVariants)
                {
                    // Tables have different structure depending on the segment
                    hasVariants = info.IsDefined(typeof(IgnoreForAttribute), false);
                }

                if (info.GetSetMethod() != null && !info.IsDefined(typeof(ComputedAttribute), false))
                {
                    dataProps.Add(info);
                }

                allProps.Add(info);
            }

            if (segProps.Count == 0)
            {
                throw new Exception($"Entity {type.FullName} is expected to be multi-table but has no {nameof(SegmentAttribute)}s set.");
            }

            if (dataProps.Count == 0)
            {
                throw new Exception($"Entity {type.FullName} has no writable columns. Remember columns with {nameof(KeyAttribute)}, {nameof(ComputedAttribute)} or {nameof(SegmentAttribute)} are read-only.");
            }

            var cacheEntry = new CacheEntry
            {
                EntityType = type,
                TablePrefix = tablePrefix,
                KeyProperty = keyProp,
                Segments = segProps.Values.ToList(),
                HasVariants = hasVariants,
                AllColumns = allProps,
                DataColumns = dataProps
            };
            BuildSqlTextTemplates(cacheEntry);

            Cache[type] = cacheEntry;

            return cacheEntry;
        }

        private static ITableInfoV1 ParseTypeAndSegments<T>(T entity)
        {
            var segments = GetSegments(entity);
            return ParseTypeAndSegments(typeof(T), segments);
        }

        private static ITableInfoV1 ParseTypeAndSegments(Type type, params string[] segments)
        {
            if (!Cache.TryGetValue(type, out var info))
            {
                info = ParsePropertiesAndAttributes(type);
            }

            var tableSuffix = GetTableSuffix(segments);

            if (info.Segments.Count != segments.Length) // TODO: Support optional segments
            {
                throw new ArgumentOutOfRangeException($"{nameof(ParseTypeAndSegments)}: Table suffix {tableSuffix} has an incorrect number of segments for type {type.Name}.");
            }

            if (info.HasVariants)
            {
                if(info.Variants.TryGetValue(tableSuffix, out var cachedVariant))
                {
                    return cachedVariant;
                }
            }

            //Type already parsed but not for this specific set of segments
            var varDataProps = new List<PropertyInfo>(info.DataColumns);
            var varAllProps = new List<PropertyInfo>(info.AllColumns);
            foreach (var prop in info.DataColumns)
            {
                var ignoreAttrs = prop.GetCustomAttributes<IgnoreForAttribute>();
                foreach (var attr in ignoreAttrs)
                {
                    // If attribute provides more segments than configured, does not match
                    if (attr.Segments.Length <= segments.Length)
                    {
                        var match = false;
                        for (int i = 0; i < attr.Segments.Length; i++)
                        {
                            match = match && (
                                attr.Segments[i] == null ||
                                attr.Segments[i] == "*" ||
                                attr.Segments[i].Equals(segments[i])
                            );
                        }

                        if (match)
                        {
                            varDataProps.Remove(prop);
                            varAllProps.Remove(prop);
                            break;
                        }
                    }
                }
            }

            var newEntry = new CacheEntry
            {
                EntityType = type,
                TablePrefix = info.TablePrefix,
                KeyProperty = info.KeyProperty,
                Segments = info.Segments,
                HasVariants = true,
                AllColumns = varAllProps,
                DataColumns = varDataProps
            };

            BuildSqlTextTemplates(newEntry);

            if (info.Variants == null)
            {
                info.Variants = new Dictionary<string, ITableInfoV1>();
            }

            var variant = new TableInfo(newEntry, tableSuffix);
            info.Variants[tableSuffix] = variant;
            return variant;
        }

        private static void BuildSqlTextTemplates(CacheEntry info)
        {
            const string COMMA = ", ";
            info.SqlSelectColumnsText = string.Join(COMMA, info.AllColumns.Select(p => p.Name));

            var dataCols = info.DataColumns.Select(p => p.Name);
            info.SqlInsertColumnNames = $" ({string.Join(COMMA, dataCols)})";
            info.SqlInsertColumnValues = $" VALUES (@{string.Join(", @", dataCols)}) ";

            var sb = new StringBuilder();
            for (int i = 0; i < info.DataColumns.Count; i++)
            {
                if (i > 0) sb.AppendLine(COMMA).Append(' ', 4);

                var name = info.DataColumns[i].Name;
                sb.Append(name).Append(" = @").Append(name);
            }
            info.SqlUpdateColumnsText = sb.ToString();
        }





        private static string[] GetSegments<T>(T entity, CacheEntry entry)
        {
            return entry.Segments.Select(s => s.GetValue(entity)?.ToString()).ToArray();
        }

        private static string GetTableSuffix<T>(T entity, CacheEntry entry)
        {
            if (entity == null) return null;

            return string.Join("_", GetSegments(entity, entry));
        }

        private static string GetTableSuffix(params string[] segments)
        {
            return string.Join("_", segments).ToUpper();
        }
    }
}

