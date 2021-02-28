using System.Collections.Generic;
using System.Data;
using Dapper;

namespace MultiTableRepository.FluentSql.Implementation
{
    internal enum SqlOperation
    {
        NONE,
        SELECT,
        INSERT,
        UPDATE,
        DELETE,
        //TODO: PROCEDURE, FUNCTION
    }

    internal class Context
    {
        public Context ParentContext { get; set; } = null;

        public IDbConnection Connection { get; set; }
        public ITableInfo TableInfo { get; set; }
        public SqlOperation Operation { get; set; } = SqlOperation.NONE;

        public string TableName { get; set; }
        public string KeyName { get; set; }
        public object EntityKey { get; set; }
        public object Entity { get; set; }

        public string Columns { get; set; }
        public string Alias { get; set; }
        public int? Limit { get; set; }
        public List<string> InnerJoins { get; set; } = new List<string>(); //TODO: LEFT, RIGHT, OUTER...
        public List<string> Where { get; set; } = new List<string>();
        public List<string> TextBeforeWhere { get; set; } = new List<string>();
        public List<string> TextAfterWhere { get; set; } = new List<string>();
        public DynamicParameters Parameters { get; } = new DynamicParameters();

        // Query
        public bool Distinct { get; set; } = false;
        public bool NoLock { get; set; } = false;
        public string OrderBy { get; set; }     //TODO: GROUP BY, HAVING
        public int? PageNumber { get; set; }
        public int PageSize { get; set; } = 10;

        // Insert
        public bool FetchNewKey { get; set; } = false;
        public string ValueClause { get; set; }
        public bool Confirm { get; set; }
    }
}
