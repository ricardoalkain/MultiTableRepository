using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using SimpleFluentSql;

namespace SimpleFluentSql.Engine
{
    public class FluentSql : IFluentSqlCommand
    {
        public static IFluentSqlCommand CreateFluentSql(string tableName, string keyColumn, IEnumerable<string> columns)
        {
            return new FluentSql(tableName, keyColumn, columns);
        }

        private readonly Context Context;


        internal const string SEP_COMMA = ", ";
        internal const string SEP_AND = " AND ";

        private FluentSql(string tableName, string keyName, IEnumerable<string> columnNames)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException(nameof(tableName));
            }

            if (string.IsNullOrEmpty(keyName))
            {
                throw new ArgumentException(nameof(keyName));
            }

            if (columnNames is null || !columnNames.Any())
            {
                throw new ArgumentNullException(nameof(columnNames));
            }

            columnNames = columnNames.Where(col => !col.Equals(keyName, StringComparison.OrdinalIgnoreCase)).AsEnumerable();
            Context = new Context(tableName, keyName, columnNames);
        }

        public IFluentSqlSelect Select()
        {
            return new FluentSqlSelect(Context);
        }

        public IFluentSqlInsert Insert()
        {
            return new FluentSqlInsert(Context);
        }

        public IFluentSqlUpdate Update()
        {
            throw new NotImplementedException();
        }

        public IFluentSqlDelete Delete()
        {
            throw new NotImplementedException();
        }
    }
}
