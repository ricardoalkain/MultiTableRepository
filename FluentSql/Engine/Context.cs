using System.Collections.Generic;

namespace SimpleFluentSql.Engine
{
    internal class Context
    {
        public Context(string tableName, string keyColumn, IEnumerable<string> columns)
        {
            TableName = tableName;
            Columns = columns;
            KeyColumn = keyColumn;
        }

        public SqlOperation Operation { get; internal set; }

        public string TableName { get; internal set; }
        public string KeyColumn { get; internal set; }
        public object EntityKey { get; internal set; }

        public IEnumerable<string> Columns { get; internal set; }

        public List<string> Where { get; } = new List<string>();
        public List<string> CustomSqlFrom { get; } = new List<string>();
        public List<string> CustomSqlWhere { get; } = new List<string>();
        public List<string> Joins { get; } = new List<string>();

        public string Alias { get; internal set; }
        public int? Limit { get; internal set; }
        public bool Distinct { get; internal set; }
        public bool NoLock { get; internal set; }
        public string OrderBy { get; internal set; }
        public int? PageNumber { get; internal set; }

        public bool? InsertReturnsNewKey { get; internal set; }
    }
}
