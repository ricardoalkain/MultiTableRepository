namespace MultiTableRepositoryTest.Extensions.FluentSql
{
    public interface IFluentSqlUpdate<T> : IFluentSqlBase<IFluentSqlUpdate<T>>, IFluentSqlWriter, IFluentWhere<IFluentSqlUpdate<T>>
    {
        IFluentSqlUpdate<T> Set(params string[] columnNames);
        IFluentSqlUpdate<T> Set(string columnName, object paramValue);

        //IFluentSqlSelect<T> FromQuery(string tableName);
    }
}