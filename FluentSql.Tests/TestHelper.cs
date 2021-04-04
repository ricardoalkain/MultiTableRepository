using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SimpleFluentSql.Engine;
using Xunit;
using Xunit.Sdk;

namespace SimpleFluentSql.Tests
{
    public static class Consts
    {
        public const string DEF_TABLE = "TEST_TABLE";
        public const string DEF_KEY = "Id";
        public const string DEF_COLUMNS = "Id, Name, Value, Date, Flag, ExtId";
        public const string DEF_DATACOLS = "Name, Value, Date";
        public const string DEF_DATAPARAMS = "@Name, @Value, @Date";
        public const string DEF_PARAMETERS = "@Id, @Name, @Value, @Date, @Flag, @ExtId";
    }

    public static class TestHelper
    {

        private static readonly SqlTableInfo defaultTableInfo;

        static TestHelper()
        {
            var splitSep = new[] { ' ', ',' };
            defaultTableInfo = new SqlTableInfo
            {
                TableName =   Consts.DEF_TABLE,
                KeyColumn =   Consts.DEF_KEY,
                WritableCols = Consts.DEF_DATACOLS.Split(splitSep, StringSplitOptions.RemoveEmptyEntries),
                Columns =  Consts.DEF_COLUMNS.Split(splitSep, StringSplitOptions.RemoveEmptyEntries),
            };
        }

        public static void AssertSameText(string expected, string actual)
        {
            //language=regex
            const string RX = @"\s+";

            var exp = Regex.Replace(expected, RX, " ").Trim();
            var act = Regex.Replace(actual, RX, " ").Trim();

            Assert.Equal(exp, act, true, true, true);
        }

        public static IFluentSqlCommand CreateFluentSql(ISqlTableInfo tableInfo = null)
        {
            return FluentSql.CreateFluentSql(tableInfo ?? defaultTableInfo);
        }
    }
}
