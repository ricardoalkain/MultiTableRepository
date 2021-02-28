using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTableRepositoryTest.Extensions.FluentSql.Implementation
{
    internal class FluentSqlUpdate<T> : BaseFluentSql<IFluentSqlUpdate<T>>, IFluentSqlUpdate<T>
    {
        internal static IFluentSqlUpdate<TEntity> Chain<TEntity>(Context context)
        {
            return new FluentSqlUpdate<TEntity>(context);
        }

        public FluentSqlUpdate(Context context) : base(context)
        {
            Instance = this;
        }

        public long Execute()
        {
            var sql = GetSql();
            return Context.Connection.Execute(sql, Context.Parameters);
        }

        public IFluentSqlSelect<T> FromQuery(string tableName)
        {
            throw new NotImplementedException();  //TODO: Chain IFluentSqlSelect
        }

        public IFluentSqlUpdate<T> Set(params string[] columnNames)
        {
            if (columnNames.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columnNames), "Can't be empty");
            }

            var str = new StringBuilder();
            foreach (var col in columnNames)
            {
                str.Append(FluentSql.SEPARATOR).Append(col).Append(" = @").Append(col);
            }
            str.Remove(0, FluentSql.SEPARATOR.Length);
            Context.Columns = str.ToString();

            return this;
        }

        public IFluentSqlUpdate<T> Set(string columnName, object paramValue)
        {
            if (!string.IsNullOrWhiteSpace(Context.Columns))
            {
                Context.Columns += FluentSql.SEPARATOR;
            }
            Context.Columns += $"{columnName} = @{columnName}";
            AddParam(columnName, paramValue);

            return this;
        }

        public override string GetSql()
        {
            var sql = new StringBuilder();

            sql.Append("UPDATE ");

            if (Context.InnerJoins.Count == 0 || string.IsNullOrEmpty(Context.Alias) || Context.EntityKey != null)
            {
                sql.Append(Context.TableName).Append(" ");
            }

            sql.AppendLine(Context.Alias);

            sql.Append("SET ");
            if (string.IsNullOrWhiteSpace(Context.Columns))
            {
                sql.AppendLine(Context.TableInfo.SqlUpdateColumnsText);
            }
            else
            {
                sql.AppendLine(Context.Columns);
            }

            if (Context.EntityKey != null)
            {
                sql.AppendLine($"WHERE {Context.KeyName} = @{Context.KeyName}");
            }
            else
            {
                if (Context.InnerJoins.Count > 0)
                {
                    sql.Append("FROM ")
                       .Append(Context.TableName)
                       .Append(" ")
                       .AppendLine(Context.Alias);

                    foreach (var join in Context.InnerJoins)
                    {
                        sql.Append("    INNER JOIN ").AppendLine(join);
                    }
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
            }

            return sql.ToString();
        }

        public IFluentSqlUpdate<T> Where(string conditions)
        {
            Context.Where.Add(conditions); //TODO: Create a BaseFluentSqlWhere<type> : BaseFluentSql
            return this;
        }
    }
}
