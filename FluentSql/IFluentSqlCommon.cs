using System.Collections.Generic;
using Dapper;

namespace SimpleFluentSql
{
    public interface IFluentSqlCommon<T>
    {
        /// <summary>
        /// Returns the final SQL script based on the previous method calls
        /// </summary>
        string GetSql();

        ///// <summary>
        ///// Reset the current parameters (conditions, joins, orders...) without modifying the
        ///// current parameters.
        ///// </summary>
        //T Clear(); //TODO: IMplement context state

        /// <summary>
        /// Inserts a new line with SQL expression/statement to the current SQL script.
        /// </summary>
        /// <param name="sql">SQL expressions or statements to be inserted.</param>
        T AppendSql(string sql);
    }

}