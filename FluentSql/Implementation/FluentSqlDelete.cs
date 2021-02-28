using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTableRepositoryTest.Extensions.FluentSql.Implementation
{
    //TODO: Update and Delete are too similar

    internal class FluentSqlDelete : BaseFluentSql<IFluentSqlDelete>, IFluentSqlDelete
    {
        internal static IFluentSqlDelete Chain(Context context)
        {
            return new FluentSqlDelete(context);
        }

        public FluentSqlDelete(Context context) : base(context)
        {
            Instance = this;
        }

        public long All()
        {
            if (!Context.Confirm)
            {
                throw new InvalidOperationException();
            }

            throw new NotImplementedException();
        }

        public long Execute()
        {
            var sql = GetSql();
            return Context.Connection.Execute(sql, Context.Parameters);
        }

        public IFluentSqlDelete Where(string conditions)
        {
            Context.Where.Add(conditions);
            return this;
        }

        public override string GetSql()
        {
            var sql = new StringBuilder();

            sql.Append("DELETE ");

            if (Context.InnerJoins.Count == 0 || string.IsNullOrEmpty(Context.Alias) || Context.EntityKey != null)
            {
                sql.Append(Context.TableName).Append(" ");
            }

            sql.AppendLine(Context.Alias);

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
    }
}
