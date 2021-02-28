using System;
using System.Collections.Generic;
using System.Reflection;

namespace MultiTableRepository.Fluent
{
    public interface ITableInfo
    {
        Type EntityType { get; }

        string TableName { get; }
        string TablePrefix { get; }
        string TableSuffix { get; }
        string KeyColumn { get; }
        IEnumerable<string> WritableColumns { get; }
        IEnumerable<string> AllColumns { get; }

        PropertyInfo KeyProperty { get; }
        IEnumerable<PropertyInfo> WritableProperties { get; }
        IEnumerable<PropertyInfo> AllProperties { get; }

        SqlParts SqlTemplates { get; }
    }

    /// <summary>
    /// Cached excerpts of SQL statements used to fast build database commands.
    /// </summary>
    public struct SqlParts
    {
        public string SelectColumns { get; set; }
        public string UpdateSetColumns { get; set; }
        public string InsertColumnNames { get; set; }
        public string InsertColumnValues { get; set; }
        public string WhereId { get; set; }
    }
}
