using System.Collections.Generic;
using Dapper;

namespace MultiTableRepository.FluentSql
{
    public interface IFluentSqlBase<T>
    {
        string GetSql();

        T Alias(string tableAlias);
        T Columns(params string[] customColumns);

        T Clear();
        T AppendSql(string sql);
        T AppendSql(string sql, params object[] values);

        T AddParam(string name, object value);
        T AddParams(IDictionary<string, object> parameters);
        T AddParams(DynamicParameters parameters);

        T Only(object entityKey);
    }

}