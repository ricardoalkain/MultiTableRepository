using Dapper;
using System;
using System.Collections.Generic;

namespace MultiTableRepository.FluentSql.Implementation
{
    internal abstract class BaseFluentSql<T> : IFluentSqlBase<T>
    {
        protected Context Context { get; set; }

        protected BaseFluentSql(Context context)
        {
            Context = context;
        }

        protected T Instance { get; set; }

        public T AddParam(string name, object value)
        {
            Context.Parameters.Add(name, value);
            return Instance;
        }

        public T AddParams(IDictionary<string, object> parameters)
        {
            Context.Parameters.AddDynamicParams(parameters);
            return Instance;
        }

        public T AddParams(DynamicParameters parameters)
        {
            Context.Parameters.AddDynamicParams(parameters);
            return Instance;
        }


        public T AppendSql(string sql)
        {
            if (Context.Where.Count == 0)
            {
                Context.TextBeforeWhere.Add(sql);
            }
            else
            {
                Context.TextAfterWhere.Add(sql);
            }
            return Instance;
        }

        public T AppendSql(string sql, params object[] values)
        {
            return AppendSql(string.Format(sql, values));
        }

        public T Clear()
        {
            throw new NotImplementedException();
        }


        public T Alias(string tableAlias)
        {
            Context.Alias = tableAlias;
            return Instance;
        }

        public T Columns(params string[] customColumns)
        {
            Context.Columns = string.Join(FluentSql.SEPARATOR, customColumns);
            return Instance;
        }

        public T Only(object entityKey)
        {
            Context.EntityKey = entityKey;
            AddParam(Context.KeyName, entityKey);
            return Instance;
        }


        public abstract string GetSql();
    }
}
