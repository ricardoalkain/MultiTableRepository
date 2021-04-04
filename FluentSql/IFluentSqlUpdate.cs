namespace SimpleFluentSql
{
    public interface IFluentSqlUpdate : IFluentSqlCommon<IFluentSqlUpdate>, IFluentSqlWhere<IFluentSqlUpdate>, IFluentSqlColumns<IFluentSqlUpdate>
    {
        //TODO: IFluentSqlSelect<T> FromQuery(string tableName); (as insert)
    }
}