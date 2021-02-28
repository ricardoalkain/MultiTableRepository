using System.Collections.Generic;

namespace MultiTableRepository.FluentSql
{
    public interface IFluentSqlSelect<T> : IFluentSqlBase<IFluentSqlSelect<T>>, IFluentWhere<IFluentSqlSelect<T>>
    {
        IFluentSqlSelect<T> Join(string table, string conditions);
        IFluentSqlSelect<T> Join(string joinClause);

        IFluentSqlSelect<T> Top(int rows);
        IFluentSqlSelect<T> Distinct();
        IFluentSqlSelect<T> WithNoLock();
        IFluentSqlSelect<T> OrderBy(params string[] columns);
        IFluentSqlSelect<T> Paging(int pageNumber, int pageSize, params string[] orderByColumns);

        IEnumerable<T> Query();
        IEnumerable<T> Query(int numberOfRows);
        T QueryOne();
    }

}