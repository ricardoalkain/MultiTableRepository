using System;
using System.Collections.Generic;
using System.Reflection;
using SimpleFluentSql;

namespace MultiTableRepository.Parser
{
    public static partial class MultiTableParser
    {
        private class CacheHeader
        {
            public Type EntityType { get; set;  }
            public string TablePrefix { get; set; }
            public string KeyColumn { get; set; }
            public IList<string> AllColumns { get; set; }
            public IList<string> WritableColumns { get; set; }
            public IList<string> SegmentColumns { get; set; }
            public PropertyInfo KeyProperty { get; set; }
            public IList<PropertyInfo> AllProperties { get; set; }
            public IList<PropertyInfo> WritableProperties { get; set; }
            public IList<PropertyInfo> SegmentProperties { get; set; }

            public bool HasVariants { get; set; }
            public Dictionary<PropertyInfo, IEnumerable<string[]>> IgnoredSegments { get; set; }
            public Dictionary<PropertyInfo, IEnumerable<string[]>> ExclusiveSegments { get; set; }

            public Dictionary<string, CacheItem> Items { get; set; } = new Dictionary<string, CacheItem>();
        }

        private class CacheItem : IMultiTableInfo
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
            public IEnumerable<string> SegmentColumns => Parent?.SegmentColumns;
            public IEnumerable<PropertyInfo> SegmentProperties => Parent?.SegmentProperties;

            public string TableName { get; set; }
            public string TableSuffix { get; set; }
            public IEnumerable<string> Columns { get; set; }
            public IEnumerable<string> WritableCols { get; set; }

            public IEnumerable<PropertyInfo> WritableProperties { get; set; }
            public IEnumerable<PropertyInfo> AllProperties { get; set; }
        }

        private static readonly Dictionary<Type, CacheHeader> Cache = new Dictionary<Type, CacheHeader>();
    }
}

