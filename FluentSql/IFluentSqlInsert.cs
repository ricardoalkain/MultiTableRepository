using System.Collections.Generic;

namespace SimpleFluentSql
{
    public interface IFluentSqlInsert : IFluentSqlCommon<IFluentSqlInsert>
    {
        //TODO: IFluentSqlSelect FromQuery(string tableName);

        /// <summary>
        /// By default INSERT returns the new ID when inserting only one entity. For multiple
        /// rows though, INSERT command will return the number of inserted rows. Use this
        /// method to get the ID of the last inserted entity instead.
        /// </summary>
        IFluentSqlInsert FetchNewKey();

        IFluentSqlSelect From(string tableName);

        IFluentSqlInsert FromSql(string sql);
    }

}