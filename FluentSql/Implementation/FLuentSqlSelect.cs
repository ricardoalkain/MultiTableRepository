using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiTableRepositoryTest.Extensions.FluentSql.Implementation
{
    internal class FLuentSqlSelect<T> : BaseFluentSql<IFluentSqlSelect<T>>, IFluentSqlSelect<T>
    {
        internal static IFluentSqlSelect<TEntity> Chain<TEntity>(Context context)
        {
            return new FLuentSqlSelect<TEntity>(context);
        }

        protected FLuentSqlSelect(Context context) : base(context)
        {
            Instance = this;
        }

        public IFluentSqlSelect<T> Distinct()
        {
            Context.Distinct = true;
            return this;
        }

        public IFluentSqlSelect<T> Join(string table, string conditions)
        {
            return Join($"{table} ON {conditions}");
        }

        public IFluentSqlSelect<T> Join(string joinClause)
        {
            Context.InnerJoins.Add(joinClause);
            return this;
        }

        public IFluentSqlSelect<T> WithNoLock()
        {
            Context.NoLock = true;
            return this;
        }

        public IFluentSqlSelect<T> OrderBy(params string[] columns)
        {
            Context.OrderBy = string.Join(FluentSql.SEPARATOR, columns);
            return this;
        }

        public IFluentSqlSelect<T> Paging(int pageNumber, int pageSize, params string[] orderByColumns)
        {
            if (orderByColumns != null)
            {
                OrderBy(orderByColumns);
            }
            Context.PageNumber = pageNumber;
            Context.PageSize = pageSize;
            return this;
        }

        public IFluentSqlSelect<T> Top(int rows)
        {
            Context.Limit = rows;
            return this;
        }

        public IFluentSqlSelect<T> Where(string conditions)
        {
            Context.Where.Add(conditions);
            return this;
        }



        public IEnumerable<T> Query()
        {
            var sql = GetSql();

            return Context.Connection.Query<T>(sql, Context.Parameters);
        }

        public IEnumerable<T> Query(int maxRows)
        {
            Context.Limit = maxRows;
            return Query();
        }

        public T QueryOne()
        {
            return Query(1).FirstOrDefault();
        }



        public override string GetSql()
        {
            var sql = new StringBuilder();

            sql.Append("SELECT ");

            if (Context.Distinct)
            {
                sql.Append("DISTINCT ");
            }

            if (Context.Limit.HasValue && Context.EntityKey == null)
            {
                sql.Append($"TOP({Context.Limit.Value}) ");
            }

            sql.AppendLine(Context.Columns);

            sql.Append("FROM ")
               .Append(Context.TableName)
               .Append(" ")
               .Append(Context.Alias); //TODO: Parse columns to include Alias (i.e. ALIAS.ColName)

            if (Context.NoLock)
            {
                sql.Append(" WITH (NOLOCK)");
            }

            sql.AppendLine();

            if (Context.EntityKey != null)
            {
                sql.AppendLine($"WHERE {Context.KeyName} = @{Context.KeyName}");
            }
            else
            {
                foreach (var join in Context.InnerJoins)
                {
                    sql.Append("    INNER JOIN ").AppendLine(join);
                }

                foreach (var line in Context.TextBeforeWhere)
                {
                    sql.AppendLine(line);
                }

                for (int i = 0; i < Context.Where.Count; i++)
                {
                    sql.Append(i == 0 ? "WHERE " : "  AND ") //TODO: OR, AND NOT, OR NOT...
                       .Append("(").Append(Context.Where[i]).AppendLine(")");
                }

                foreach (var line in Context.TextAfterWhere)
                {
                    sql.AppendLine(line);
                }

                if (!string.IsNullOrEmpty(Context.OrderBy))
                {
                    sql.Append("ORDER BY ").AppendLine(Context.OrderBy);

                    if (Context.PageNumber >= 0)
                    {
                        sql.AppendLine($"OFFSET {Context.PageNumber} ROWS")
                           .AppendLine($"FETCH NEXT {Context.PageSize} ROWS ONLY");
                    }
                }
                else if (Context.PageNumber.HasValue)
                {
                    throw new ArgumentNullException(nameof(OrderBy), "Paging is not allowed without an ORDER BY clause.");
                }
            }

            return sql.ToString();
        }
    }
}
