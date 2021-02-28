using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dapper.Contrib.Extensions;
using MultiTableRepository.Attributes;
using MultiTableRepository.Fluent;
using MultiTableRepository.Parser;
using Xunit;

namespace Repository.Tests
{
    public class ParserTests
    {
        private const string TABLE_PREFIX = "TABLE";
        private const string SEG_SEP = "_";
        private const int SEGMENT_INDEX_COUNTRY = 0;
        private const int SEGMENT_INDEX_COMMODITY = 1;
        private const int SEGMENT_INDEX_PORTFOLIO = 2;

        private class TestTableInfo : ITableInfo
        {
            public Type EntityType { get; set; }
            public string TablePrefix { get; set; }
            public string TableSuffix { get; set; }
            public PropertyInfo KeyProperty { get; set; }
            public IEnumerable<PropertyInfo> WritableProperties { get; set; }
            public IEnumerable<PropertyInfo> AllProperties { get; set; }
            public SqlParts SqlTemplates { get; set; }

            public string TableName => $"{TablePrefix}_{TableSuffix}";
            public string KeyColumn => KeyProperty?.Name;
            public IEnumerable<string> AllColumns { get; set; }
            public IEnumerable<string> WritableColumns { get; set; }
        }

        [MultiTable(TABLE_PREFIX)]
        private class TestModel
        {
            [Key]
            public int Id { get; set; }

            [Segment(SEGMENT_INDEX_COUNTRY)]
            public string Country { get; set; }

            [Segment(SEGMENT_INDEX_COMMODITY)]
            public string Commodity { get; set; }

            [Segment(SEGMENT_INDEX_PORTFOLIO)]
            public string Portfolio { get; set; }

            public string Name { get; set; }

            public double Value { get; set; }

            [ExclusiveFor(null, "GAS")]
            public string GasType { get; set; }

            public double Ratio { get; set; }

            [ExclusiveFor(null, null, "B2B")]
            [IgnoreFor(null, "pOwEr")]
            public double ValueB2B { get; set; }

            [IgnoreFor("DE", "gas")]
            public int NoGermanGas { get; set; }

            public DateTime CreatedOn { get; set; }
        }

        [Theory]
        [InlineData("be", "power", "b2b", new[] { nameof(TestModel.GasType), nameof(TestModel.ValueB2B), })]
        [InlineData("be", "power", "b2c", new[] { nameof(TestModel.GasType), nameof(TestModel.ValueB2B), })]
        [InlineData("be", "power", "ge",  new[] { nameof(TestModel.GasType), nameof(TestModel.ValueB2B), })]

        [InlineData("be", "gas",   "b2b", null)]
        [InlineData("be", "gas",   "b2c", new[] { nameof(TestModel.ValueB2B), })]
        [InlineData("be", "gas",   "ge",  new[] { nameof(TestModel.ValueB2B), })]

        [InlineData("fr", "power", "b2b", new[] { nameof(TestModel.GasType), nameof(TestModel.ValueB2B), })]
        [InlineData("fr", "power", "b2c", new[] { nameof(TestModel.GasType), nameof(TestModel.ValueB2B), })]
        [InlineData("fr", "power", "ge",  new[] { nameof(TestModel.GasType), nameof(TestModel.ValueB2B), })]

        [InlineData("fr", "gas",   "b2b", null)]
        [InlineData("fr", "gas",   "b2c", new[] { nameof(TestModel.ValueB2B), })]
        [InlineData("fr", "gas",   "ge",  new[] { nameof(TestModel.ValueB2B), })]

        [InlineData("de", "power", "b2b", new[] { nameof(TestModel.GasType), nameof(TestModel.ValueB2B), })]
        [InlineData("de", "power", "b2c", new[] { nameof(TestModel.GasType), nameof(TestModel.ValueB2B), })]
        [InlineData("de", "power", "ge",  new[] { nameof(TestModel.GasType), nameof(TestModel.ValueB2B), })]

        [InlineData("de", "gas",   "b2b", new[] { nameof(TestModel.NoGermanGas), })]
        [InlineData("de", "gas",   "b2c", new[] { nameof(TestModel.NoGermanGas), nameof(TestModel.ValueB2B), })]
        [InlineData("de", "gas",   "ge",  new[] { nameof(TestModel.NoGermanGas), nameof(TestModel.ValueB2B), })]
        public void GetTableInformation_ValidSegments_ReturnsTableInfo(
            string country, string commodity, string portfolio, string[] ignoredColumns = null)
        {
            var segments = new string[] { country, commodity, portfolio };
            var expected = MockTableInfo(segments, ignoredColumns);

            var result = MultiTableParserV2.GetTableInformation<TestModel>(segments);

            Assert.NotNull(result);
            Assert.NotNull(result.TableName);
            Assert.Equal(expected.EntityType, result.EntityType);
            Assert.Equal(expected.TableName, result.TableName);
            Assert.Equal(expected.TablePrefix, result.TablePrefix);
            Assert.Equal(expected.TableSuffix, result.TableSuffix);
            Assert.Equal(expected.KeyProperty, result.KeyProperty);
            Assert.Equal(expected.KeyColumn, result.KeyColumn);
            Assert.Equal(expected.AllProperties, result.AllProperties);
            Assert.Equal(expected.WritableProperties, result.WritableProperties);
            Assert.Equal(expected.AllColumns, result.AllColumns);
            Assert.Equal(expected.WritableColumns, result.WritableColumns);
        }

        #region Helper methods

        private ITableInfo MockTableInfo(string[] segments, string[] ignoredColumns)
        {
            var type = typeof(TestModel);

            var allProps = type.GetProperties().ToList();
            allProps.RemoveAll(p => ignoredColumns?.Contains(p.Name) ?? false);

            var wriProps = allProps.ToList();
            wriProps.RemoveAll(p => p.Name == nameof(TestModel.Id) ||
                                    p.Name == nameof(TestModel.Country) ||
                                    p.Name == nameof(TestModel.Commodity) ||
                                    p.Name == nameof(TestModel.Portfolio));

            return new TestTableInfo
            {
                EntityType = typeof(TestModel),
                AllProperties = allProps,
                WritableProperties = wriProps,
                KeyProperty = allProps.FirstOrDefault(p => p.Name == nameof(TestModel.Id)),
                TablePrefix = TABLE_PREFIX,
                TableSuffix = GetSuffix(segments),
                AllColumns = allProps.Select(p => p.Name).ToList(),
                WritableColumns = wriProps.Select(p => p.Name).ToList()
            };
        }

        private string GetSuffix(params string[] segments)
        {
            return string.Join(SEG_SEP, segments);
        }

        #endregion
    }
}
