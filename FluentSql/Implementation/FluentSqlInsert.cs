using System;
using System.Collections.Generic;
using System.Text;
using Dapper;

namespace MultiTableRepository.Fluent.Implementation
{
    internal class FluentSqlInsert<T> : BaseFluentSql<IFluentSqlInsert<T>>, IFluentSqlInsert<T>
    {
        internal static IFluentSqlInsert<TEntity> Chain<TEntity>(Context context)
        {
            return new FluentSqlInsert<TEntity>(context);
        }

        protected FluentSqlInsert(Context context) : base(context)
        {
            Instance = this;
        }

        public long Execute()
        {
            Context.FetchNewKey = false;
            var sql = GetSql();
            return Context.Connection.Execute(sql, Context.Parameters);
        }

        public long FromEntities(IEnumerable<T> entities)
        {
            Context.FetchNewKey = false;
            var sql = GetSql();
            long count = 0;
            foreach (var item in entities) //TODO: BULK insert
            {
                count += Context.Connection.Execute(sql, item);
            }
            return count;
        }

        public IFluentSqlSelect<T> FromQuery(string tableName)
        {
            throw new NotImplementedException(); //TODO: Chains SELECT command
        }

        public long ExecuteAndGetId()
        {
            Context.FetchNewKey = true;
            var sql = GetSql();
            return Context.Connection.QuerySingle<long>(sql, Context.Parameters);
        }

        public override string GetSql()
        {
            var sql = new StringBuilder();

            sql.Append("INSERT INTO ")
               .Append(Context.TableName)
               .Append(Context.Alias)  //TODO: Parse columns to include Alias (i.e. ALIAS.ColName)
               .Append(" ");

            sql.AppendLine(Context.Columns)
               .AppendLine(Context.ValueClause);

            if (Context.FetchNewKey)
            {
                sql.AppendLine(";SELECT SCOPE_IDENTITY() AS NewKey");
            }

            return sql.ToString();
        }
    }
}
