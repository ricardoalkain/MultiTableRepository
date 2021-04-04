using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SimpleFluentSql.Engine;
using Xunit;

namespace SimpleFluentSql.Tests
{
    public class FluentSqlInsertTests
    {

        public FluentSqlInsertTests()
        {
        }

        [Fact]
        public void Select_AllDefault()
        {
            var expected = $"INSERT INTO {Consts.DEF_TABLE} ({Consts.DEF_DATACOLS}) " +
                           $"VALUES({Consts.DEF_DATAPARAMS})";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Insert()
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }
    }
}
