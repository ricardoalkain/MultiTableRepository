using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;

namespace SimpleFluentSql.Engine
{
    internal abstract class FluentSqlBase<T> : IFluentSqlCommon<T>, IFluentSqlWhere<T>, IFluentSqlColumns<T>
    {
        protected T Instance;

        protected Context Context { get; private set; }

        public FluentSqlBase(Context context)
        {
            Context = context;
        }

        public abstract string GetSql();

        public T AppendSql(string sql)
        {
            if (Context.Where.Any() || Context.EntityKey != null)
            {
                Context.CustomSqlWhere.Add(sql);
            }
            else
            {
                Context.CustomSqlFrom.Add(sql);
            }
            return Instance;
        }

        public T Alias(string tableAlias)
        {
            Context.Alias = tableAlias;
            return Instance;
        }

        public T Top(int maxRows)
        {
            Context.Limit = maxRows;
            return Instance;
        }

        public T InnerJoin(string table, params string[] conditions)
        {
            return InnerJoin($"{table} ON {string.Join(FluentSql.SEP_AND, conditions)}");
        }

        public T InnerJoin(string joinClause)
        {
            Context.Joins.Add($"INNER JOIN {joinClause}");
            return Instance;
        }

        public T Where(params string[] conditions)
        {
            Context.Where.AddRange(conditions);
            return Instance;
        }

        public T SetColumn(string columnName, string sqlExpression)
        {
            throw new NotImplementedException();
        }

        public T SkipColumn(string columnName)
        {
            throw new NotImplementedException();
        }

        public T ClearColumns()
        {
            throw new NotImplementedException();
        }

        public T ResetColumns()
        {
            Context.ResetColumns();
            return Instance;
        }

        public T Only(object key)
        {
            Context.EntityKey = key;
            return Instance;
        }

        #region Helpers

        /// <summary>
        /// Flats a list of string into a single string, applying prefixes and suffixes if given.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        protected string ListToText<TItem>(IEnumerable<TItem> list, string prefix = null, string suffix = null, string separator = FluentSql.SEP_COMMA)
        {
            if (list == null || !list.Any())
            {
                return string.Empty;
            }

            var sep = $"{suffix}{separator}{prefix}";

            return $"{prefix}{string.Join(sep, list)}{suffix}";
        }

        #endregion
    }
}
