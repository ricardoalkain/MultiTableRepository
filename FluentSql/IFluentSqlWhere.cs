using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleFluentSql
{
    public interface IFluentSqlWhere<T>
    {
        T Only(object key);

        T Alias(string tableAlias);

        T Top(int maxRows);

        T InnerJoin(string table, params string[] conditions);

        T InnerJoin(string joinClause);

        T Where(params string[] conditions);
    }
}
