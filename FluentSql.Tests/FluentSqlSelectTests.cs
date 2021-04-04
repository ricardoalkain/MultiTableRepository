using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SimpleFluentSql.Engine;
using Xunit;

namespace SimpleFluentSql.Tests
{
    public class FluentSqlSelectTests
    {
        public FluentSqlSelectTests()
        {
        }

        [Fact]
        public void Select_AllDefault()
        {
            var expected = $"SELECT {Consts.DEF_COLUMNS} " +
                $"FROM {Consts.DEF_TABLE}";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_CustomColumns()
        {
            var expected = "SELECT COL1, COL2, GETDATE() AS DATE " +
                $"FROM {Consts.DEF_TABLE}";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Columns("Col1", "Col2, GETDATE() AS Date")
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_Top()
        {
            var expected = "SELECT TOP(100) Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE}";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(100)
                               .Columns("Name", "Value", "Date")
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_Distinct()
        {
            var expected = "SELECT DISTINCT TOP(100) Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE}";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(100)
                               .Distinct()
                               .Columns("Name", "Value", "Date")
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_Alias()
        {
            var expected = "SELECT DISTINCT TOP(100) Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE} TT";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(100)
                               .Distinct()
                               .Columns("Name", "Value", "Date")
                               .Alias("TT")
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_WithNoLock()
        {
            var expected = "SELECT DISTINCT TOP(100) Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE} TT WITH (NOLOCK)";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(100)
                               .Distinct()
                               .Columns("Name", "Value", "Date")
                               .Alias("TT")
                               .WithNoLock()
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_InnerJoin_SingleJoin()
        {
            var expected = "SELECT DISTINCT TOP(100) Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE} TT WITH (NOLOCK) " +
                "INNER JOIN Table2 T2 ON TT.Id = T2.Id";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(100)
                               .Distinct()
                               .Columns("Name", "Value", "Date")
                               .Alias("TT")
                               .WithNoLock()
                               .InnerJoin("Table2 T2", "TT.Id = T2.Id")
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_InnerJoin_MultipleJoin()
        {
            var expected = "SELECT DISTINCT TOP(100) Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE} TT WITH (NOLOCK) " +
                "INNER JOIN Table2 T2 ON TT.Id = T2.Id " +
                "INNER JOIN Table3 T3 ON T3.Date < TT.Date AND T3.Id = TT.ExtId";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(100)
                               .Distinct()
                               .Columns("name", "value", "date")
                               .Alias("tt")
                               .WithNoLock()
                               .InnerJoin("table2 t2 on tt.id = t2.id")
                               .InnerJoin("table3 t3", "t3.date < tt.date", "t3.id = tt.extid")
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_CustomTextBeforeWhere()
        {
            var expected = "SELECT DISTINCT TOP(100) Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE} TT WITH (NOLOCK) " +
                "INNER JOIN Table2 T2 ON TT.Id = T2.Id " +
                "INNER JOIN Table3 T3 ON T3.Date < TT.Date AND T3.Id = TT.ExtId " +
                "LEFT JOIN Table4 T4 ON T4.Value = TT.Value";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(100)
                               .Distinct()
                               .Columns("name", "value", "date")
                               .Alias("tt")
                               .WithNoLock()
                               .InnerJoin("table2 t2 on tt.id = t2.id")
                               .InnerJoin("table3 t3", "t3.date < tt.date", "t3.id = tt.extid")
                               .AppendSql("left join table4 t4 on t4.value = tt.value")
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_WhereSingle()
        {
            var expected = "SELECT DISTINCT TOP(100) Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE} TT WITH (NOLOCK) " +
                "INNER JOIN Table2 T2 ON TT.Id = T2.Id " +
                "INNER JOIN Table3 T3 ON T3.Date < TT.Date AND T3.Id = TT.ExtId " +
                "LEFT JOIN Table4 T4 ON T4.Value = TT.Value " +
                "WHERE (Date < GETDATE())";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(100)
                               .Distinct()
                               .Columns("name", "value", "date")
                               .Alias("tt")
                               .WithNoLock()
                               .InnerJoin("table2 t2 on tt.id = t2.id")
                               .InnerJoin("table3 t3", "t3.date < tt.date", "t3.id = tt.extid")
                               .AppendSql("left join table4 t4 on t4.value = tt.value")
                               .Where("date < getdate()")
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_MutipleWhere()
        {
            var expected = "SELECT DISTINCT TOP(100) Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE} TT WITH (NOLOCK) " +
                "INNER JOIN Table2 T2 ON TT.Id = T2.Id " +
                "INNER JOIN Table3 T3 ON T3.Date < TT.Date AND T3.Id = TT.ExtId " +
                "LEFT JOIN Table4 T4 ON T4.Value = TT.Value " +
                "WHERE (Date < GETDATE()) " +
                "AND (Name IS NOT NULL) " +
                "AND (Flag = 1)";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(100)
                               .Distinct()
                               .Columns("name", "value", "date")
                               .Alias("tt")
                               .WithNoLock()
                               .InnerJoin("table2 t2 on tt.id = t2.id")
                               .InnerJoin("table3 t3", "t3.date < tt.date", "t3.id = tt.extid")
                               .AppendSql("left join table4 t4 on t4.value = tt.value")
                               .Where("date < getdate()", "name is not null")
                               .Where("flag = 1")
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_CustomTextAfterWhere()
        {
            var expected = "SELECT DISTINCT TOP(100) Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE} TT WITH (NOLOCK) " +
                "INNER JOIN Table2 T2 ON TT.Id = T2.Id " +
                "INNER JOIN Table3 T3 ON T3.Date < TT.Date AND T3.Id = TT.ExtId " +
                "LEFT JOIN Table4 T4 ON T4.Value = TT.Value " +
                "WHERE (Date < GETDATE()) " +
                "AND (Name IS NOT NULL) " +
                "AND (Flag = 1) " +
                "OR TT.Value > 0";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(100)
                               .Distinct()
                               .Columns("name", "value", "date")
                               .Alias("tt")
                               .WithNoLock()
                               .InnerJoin("table2 t2 on tt.id = t2.id")
                               .InnerJoin("table3 t3", "t3.date < tt.date", "t3.id = tt.extid")
                               .AppendSql("left join table4 t4 on t4.value = tt.value")
                               .Where("date < getdate()", "name is not null")
                               .Where("flag = 1")
                               .AppendSql("or tt.value > 0")
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_OrderBy()
        {
            var expected = "SELECT DISTINCT TOP(100) Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE} TT WITH (NOLOCK) " +
                "INNER JOIN Table2 T2 ON TT.Id = T2.Id " +
                "INNER JOIN Table3 T3 ON T3.Date < TT.Date AND T3.Id = TT.ExtId " +
                "LEFT JOIN Table4 T4 ON T4.Value = TT.Value " +
                "WHERE (Date < GETDATE()) " +
                "AND (Name IS NOT NULL) " +
                "AND (Flag = 1) " +
                "OR TT.Value > 0 " +
                "ORDER BY Date DESC, NAME";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(100)
                               .Distinct()
                               .Columns("name", "value", "date")
                               .Alias("tt")
                               .WithNoLock()
                               .InnerJoin("table2 t2 on tt.id = t2.id")
                               .InnerJoin("table3 t3", "t3.date < tt.date", "t3.id = tt.extid")
                               .AppendSql("left join table4 t4 on t4.value = tt.value")
                               .Where("date < getdate()", "name is not null")
                               .Where("flag = 1")
                               .AppendSql("or tt.value > 0")
                               .OrderBy("value")
                               .OrderBy("date desc", "name") // Only last OrderBy call should be rendered
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_Paging()
        {
            var expected = "SELECT DISTINCT Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE} TT WITH (NOLOCK) " +
                "INNER JOIN Table2 T2 ON TT.Id = T2.Id " +
                "INNER JOIN Table3 T3 ON T3.Date < TT.Date AND T3.Id = TT.ExtId " +
                "LEFT JOIN Table4 T4 ON T4.Value = TT.Value " +
                "WHERE (Date < GETDATE()) " +
                "AND (Name IS NOT NULL) " +
                "AND (Flag = 1) " +
                "OR TT.Value > 0 " +
                "ORDER BY Date DESC, NAME " +
                "OFFSET 200 ROWS " +
                "FETCH NEXT 100 ROWS ONLY";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(100)
                               .Distinct()
                               .Columns("name", "value", "date")
                               .Alias("tt")
                               .WithNoLock()
                               .InnerJoin("table2 t2 on tt.id = t2.id")
                               .InnerJoin("table3 t3", "t3.date < tt.date", "t3.id = tt.extid")
                               .AppendSql("left join table4 t4 on t4.value = tt.value")
                               .Where("date < getdate()", "name is not null")
                               .Where("flag = 1")
                               .AppendSql("or tt.value > 0")
                               .OrderBy("value")
                               .OrderBy("date desc", "name")
                               .Page(3) // Should use last Top and OrderBy calls
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_PagingWithSize()
        {
            var expected = "SELECT DISTINCT Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE} TT WITH (NOLOCK) " +
                "INNER JOIN Table2 T2 ON TT.Id = T2.Id " +
                "INNER JOIN Table3 T3 ON T3.Date < TT.Date AND T3.Id = TT.ExtId " +
                "LEFT JOIN Table4 T4 ON T4.Value = TT.Value " +
                "WHERE (Date < GETDATE()) " +
                "AND (Name IS NOT NULL) " +
                "AND (Flag = 1) " +
                "OR TT.Value > 0 " +
                "ORDER BY Date DESC, NAME " +
                "OFFSET 100 ROWS " +
                "FETCH NEXT 50 ROWS ONLY";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(100) // Should be ignored
                               .Distinct()
                               .Columns("name", "value", "date")
                               .Alias("tt")
                               .WithNoLock()
                               .InnerJoin("table2 t2 on tt.id = t2.id")
                               .InnerJoin("table3 t3", "t3.date < tt.date", "t3.id = tt.extid")
                               .AppendSql("left join table4 t4 on t4.value = tt.value")
                               .Where("date < getdate()", "name is not null")
                               .Where("flag = 1")
                               .AppendSql("or tt.value > 0")
                               .OrderBy("value")
                               .OrderBy("date desc", "name")
                               .Page(3, 50) // Should use last OrderBy call
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_PagingWithOrderNoSize()
        {
            var expected = "SELECT DISTINCT Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE} TT WITH (NOLOCK) " +
                "INNER JOIN Table2 T2 ON TT.Id = T2.Id " +
                "INNER JOIN Table3 T3 ON T3.Date < TT.Date AND T3.Id = TT.ExtId " +
                "LEFT JOIN Table4 T4 ON T4.Value = TT.Value " +
                "WHERE (Date < GETDATE()) " +
                "AND (Name IS NOT NULL) " +
                "AND (Flag = 1) " +
                "OR TT.Value > 0 " +
                "ORDER BY Name DESC, Value ASC " +
                "OFFSET 100 ROWS " +
                "FETCH NEXT 50 ROWS ONLY";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(50)
                               .Distinct()
                               .Columns("name", "value", "date")
                               .Alias("tt")
                               .WithNoLock()
                               .InnerJoin("table2 t2 on tt.id = t2.id")
                               .InnerJoin("table3 t3", "t3.date < tt.date", "t3.id = tt.extid")
                               .AppendSql("left join table4 t4 on t4.value = tt.value")
                               .Where("date < getdate()", "name is not null")
                               .Where("flag = 1")
                               .AppendSql("or tt.value > 0")
                               .OrderBy("value") // Should be ignored
                               .OrderBy("date desc", "name") // Should be ignored
                               .Page(3, "name desc", "value asc") // Should use last Top call
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_PagingWithOrderAndSize()
        {
            var expected = "SELECT DISTINCT Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE} TT WITH (NOLOCK) " +
                "INNER JOIN Table2 T2 ON TT.Id = T2.Id " +
                "INNER JOIN Table3 T3 ON T3.Date < TT.Date AND T3.Id = TT.ExtId " +
                "LEFT JOIN Table4 T4 ON T4.Value = TT.Value " +
                "WHERE (Date < GETDATE()) " +
                "AND (Name IS NOT NULL) " +
                "AND (Flag = 1) " +
                "OR TT.Value > 0 " +
                "ORDER BY Name DESC, Value ASC " +
                "OFFSET 80 ROWS " +
                "FETCH NEXT 40 ROWS ONLY";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(50) // Should be ignored
                               .Distinct()
                               .Columns("name", "value", "date")
                               .Alias("tt")
                               .WithNoLock()
                               .InnerJoin("table2 t2 on tt.id = t2.id")
                               .InnerJoin("table3 t3", "t3.date < tt.date", "t3.id = tt.extid")
                               .AppendSql("left join table4 t4 on t4.value = tt.value")
                               .Where("date < getdate()", "name is not null")
                               .Where("flag = 1")
                               .AppendSql("or tt.value > 0")
                               .OrderBy("value") // Should be ignored
                               .OrderBy("date desc", "name") // Should be ignored
                               .Page(3, 40, "name desc", "value asc")
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }

        [Fact]
        public void Select_OnlyOne()
        {
            var expected = "SELECT Name, Value, Date " +
                $"FROM {Consts.DEF_TABLE} TT WITH (NOLOCK) " +
                "INNER JOIN Table2 T2 ON TT.Id = T2.Id " +
                "INNER JOIN Table3 T3 ON T3.Date < TT.Date AND T3.Id = TT.ExtId " +
                "LEFT JOIN Table4 T4 ON T4.Value = TT.Value " +
                $"WHERE {Consts.DEF_KEY} = @{Consts.DEF_KEY} " +
                "OR TT.Value > 0";

            var fluent = TestHelper.CreateFluentSql();
            var result = fluent.Select()
                               .Top(100)
                               .Distinct()
                               .Columns("name", "value", "date")
                               .Alias("tt")
                               .WithNoLock()
                               .InnerJoin("table2 t2 on tt.id = t2.id")
                               .InnerJoin("table3 t3", "t3.date < tt.date", "t3.id = tt.extid")
                               .AppendSql("left join table4 t4 on t4.value = tt.value")
                               .Where("date < getdate()", "name is not null")
                               .Where("flag = 1")
                               .AppendSql("or tt.value > 0")
                               .Only(1234)
                               .GetSql();

            Assert.NotNull(result);
            TestHelper.AssertSameText(expected, result);
        }
    }
}
