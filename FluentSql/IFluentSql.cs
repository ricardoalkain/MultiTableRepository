using System.Collections.Generic;

namespace MultiTableRepositoryTest.Extensions.FluentSql
{
    public interface IFluentSql<T>
    {
        /// <summary>
        /// Builds a SQL SELECT statement using the current table in the FROM clause
        /// and the default list of coulumns according to the properties from the
        /// underlying <see cref="T"/> object.
        /// </summary>
        IFluentSqlSelect<T> Select();

        IFluentSqlInsert<T> Insert();
        IFluentSqlUpdate<T> Update();
        IFluentSqlDelete Delete();
    }

    public interface IFluentSqlAutoExec<T> : IFluentSql<T>
    {
        IEnumerable<T> Select(params string[] conditions);
        bool DeleteIt();
        bool UpdateIt();
        bool InsertIt();
        long InsertAndGetId();
    }

}