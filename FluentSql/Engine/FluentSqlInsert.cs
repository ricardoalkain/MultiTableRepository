using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleFluentSql.Engine
{
    internal class FluentSqlInsert : FluentSqlBase<IFluentSqlInsert>, IFluentSqlInsert
    {
        public FluentSqlInsert(Context context) : base(context)
        {
            Instance = this;
        }

        public IFluentSqlInsert FetchNewKey()
        {
            Context.InsertReturnsNewKey = true;
            return Instance;
        }

        public IFluentSqlSelect From(string tableName)
        {
            throw new NotImplementedException();
        }

        public IFluentSqlInsert FromSql(string sql)
        {
            Context.CustomSqlFrom.Add(sql);
            return Instance;
        }

        public override string GetSql()
        {
            var sql = new StringBuilder();

            sql.Append("INSERT INTO ")
               .Append(Context.TableName)
               .Append(' ')
               .Append(Context.Alias);  //TODO: Parse columns to include Alias (i.e. ALIAS.ColName)

            sql.Append(" (")
               .Append(string.Join(FluentSql.SEP_COMMA, Context.WritableCols))
               .AppendLine(")");

            if (Context.CustomSqlFrom.Any())
            {
                foreach (var line in Context.CustomSqlFrom)
                {
                    sql.AppendLine(line);
                }
            }
            else
            {
                sql.Append(" VALUES (")
                   .Append(string.Join(FluentSql.SEP_COMMA, Context.DataColumnValues))
                   .AppendLine(")");
            }

            if (Context.InsertReturnsNewKey == true)
            {
                sql.AppendLine(";SELECT SCOPE_IDENTITY() AS NewKey");
            }

            return sql.ToString();
        }
    }
}
