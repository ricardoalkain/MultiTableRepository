using System.Collections.Generic;

namespace MultiTableRepositoryTest.Extensions.FluentSql
{
    public interface IFluentSqlInsert<T> : IFluentSqlBase<IFluentSqlInsert<T>>, IFluentSqlWriter
    {
        //IFluentSqlSelect<T> FromQuery(string tableName);
        long FromEntities(IEnumerable<T> entities);
        long ExecuteAndGetId();
    }

}