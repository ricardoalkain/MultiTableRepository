using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace SimpleFluentSql
{
    public interface IFluentSqlCommand
    {
        /// <summary>
        /// Builds a SQL SELECT statement using the current table in the FROM clause
        /// and the default list of coulumns.
        /// </summary>
        IFluentSqlSelect Select();

        /// <summary>
        /// Builds a SQL INSERT statement using the current table in the INTO clause
        /// and the default list of writable coulumns.
        /// </summary>
        IFluentSqlInsert Insert();

        /// <summary>
        /// Builds a SQL SELECT statement using the current table in the FROM clause
        /// and the default list of coulumns.
        /// </summary>
        IFluentSqlUpdate Update();

        /// <summary>
        /// Builds a SQL SELECT statement using the current table in the FROM clause
        /// and the default list of coulumns.
        /// </summary>
        IFluentSqlDelete Delete();
    }

}