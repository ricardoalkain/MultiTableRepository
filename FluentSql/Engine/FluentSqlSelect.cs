using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;

namespace SimpleFluentSql.Engine
{
    internal class FluentSqlSelect : FluentSqlBase<IFluentSqlSelect>, IFluentSqlSelect
    {
        private string CustomColumns;

        public FluentSqlSelect(Context context) : base(context)
        {
            Context.Operation = SqlOperation.SELECT;
            Instance = this;
        }

        public IFluentSqlSelect Distinct()
        {
            Context.Distinct = true;
            return Instance;
        }

        public IFluentSqlSelect Columns(params string[] customColumns)
        {
            CustomColumns = string.Join(FluentSql.SEP_COMMA, customColumns);
            return Instance;
        }

        public IFluentSqlSelect WithNoLock()
        {
            Context.NoLock = true;
            return Instance;
        }

        public IFluentSqlSelect OrderBy(params string[] columns)
        {
            Context.OrderBy = string.Join(FluentSql.SEP_COMMA, columns);
            return Instance;
        }

        public IFluentSqlSelect Page(int pageNumber)
        {
            return Page(pageNumber, 0, null);
        }

        public IFluentSqlSelect Page(int pageNumber, int pageSize)
        {
            return Page(pageNumber, pageSize, null);
        }

        public IFluentSqlSelect Page(int pageNumber, params string[] orderByColumns)
        {
            return Page(pageNumber, 0, orderByColumns);
        }

        public IFluentSqlSelect Page(int pageNumber, int pageSize, params string[] orderByColumns)
        {
            if (orderByColumns != null && orderByColumns.Length > 0)
            {
                OrderBy(orderByColumns);
            }

            if (pageSize > 0)
            {
                Top(pageSize);
            }

            Context.PageNumber = pageNumber;
            return Instance;
        }




        public override string GetSql()
        {
            var sql = new StringBuilder();

            sql.Append("SELECT ");

            if (Context.EntityKey == null)
            {
                if (Context.Distinct)
                {
                    sql.Append("DISTINCT ");
                }

                if (Context.Limit.HasValue && !Context.PageNumber.HasValue)
                {
                    sql.Append($"TOP({Context.Limit.Value}) ");
                }
            }

            if (string.IsNullOrWhiteSpace(CustomColumns))
            {
                IEnumerable<string> columns;

                if (string.IsNullOrWhiteSpace(Context.Alias))
                {
                    columns = Context.Columns.Select(col => $"{Context.Alias}.{col}").AsEnumerable();
                    sql.Append(Context.Alias).Append('.');
                }
                else
                {
                    columns = Context.Columns;
                }

                sql.AppendLine(string.Join(FluentSql.SEP_COMMA, columns));
            }
            else
            {
                sql.AppendLine(CustomColumns);
            }

            sql.Append("FROM ")
               .Append(Context.TableName)
               .Append(" ")
               .Append(Context.Alias);

            if (Context.NoLock)
            {
                sql.Append(" WITH (NOLOCK)");
            }

            sql.AppendLine();

            foreach (var join in Context.Joins)
            {
                sql.Append(' ', 5).AppendLine(join);
            }

            foreach (var line in Context.CustomSqlFrom)
            {
                sql.AppendLine(line);
            }

            if (Context.EntityKey == null)
            {
                for (int i = 0; i < Context.Where.Count; i++)
                {
                    sql.Append(i == 0 ? "WHERE " : "  AND ") //TODO: OR, AND NOT, OR NOT...
                        .Append("(").Append(Context.Where[i]).AppendLine(")");
                }

                foreach (var line in Context.CustomSqlWhere)
                {
                    sql.AppendLine(line);
                }

                if (!string.IsNullOrEmpty(Context.OrderBy))
                {
                    sql.Append("ORDER BY ").AppendLine(Context.OrderBy);

                    if (Context.PageNumber.HasValue)
                    {
                        sql.AppendLine($"OFFSET {(Context.PageNumber - 1) * Context.Limit} ROWS")
                            .AppendLine($"FETCH NEXT {Context.Limit} ROWS ONLY");
                    }
                }
                else if (Context.PageNumber.HasValue)
                {
                    throw new ArgumentNullException(nameof(OrderBy), "Paging is not allowed without an ORDER BY clause.");
                }
            }
            else
            {
                // Should throw an exception if some of the clauses exists together with EntityKey instead of just ignoring it?
                sql.AppendLine($"WHERE {Context.KeyColumn} = @{Context.KeyColumn}");

                foreach (var line in Context.CustomSqlWhere)
                {
                    sql.AppendLine(line);
                }
            }

            return sql.ToString();
        }
    }
}