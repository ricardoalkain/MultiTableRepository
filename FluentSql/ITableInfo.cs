using System;
using System.Collections.Generic;
using System.Reflection;

namespace MultiTableRepository.FluentSql
{
    public interface ITableInfo
    {
        Type EntityType { get; set; }

        string TableName { get; set; }
        string TablePrefix { get; set; }
        string TableSuffix { get; set; }
        string KeyColumn { get; set; }
        IEnumerable<string> AllColumns { get; set; }
        IEnumerable<string> WritableColumns { get; set; }

        PropertyInfo KeyProp { get; set; }
        IEnumerable<PropertyInfo> SegmentProps { get; set; }
        IEnumerable<PropertyInfo> DataProps { get; set; }
        IEnumerable<PropertyInfo> AllProps { get; set; }

        SqlParts SqlTemplates { get; set; }
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
