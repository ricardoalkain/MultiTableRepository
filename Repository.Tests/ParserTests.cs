using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MultiTableRepository.Fluent;
using MultiTableRepository.Parser;
using Xunit;

namespace Repository.Tests
{
    public partial class ParserTests
    {
        public const string TABLE_PREFIX = "TABLE";
        public const string SEG_SEP = "_";

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

        [Theory]
        [ClassData(typeof(SegmentAndVariantsDataProvider))]
        public void GetTableInformation_SegmenstsWithVariants(string country, string commodity, string portfolio, string[] ignore = null)
        {
            var segments = new string[] { country, commodity, portfolio };
            var expected = MockTableInfo<TestModelWithVariants>(segments, ignore);

            var result = MultiTableParserV2.GetTableInformation<TestModelWithVariants>(segments);

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


        [Theory]
        [ClassData(typeof(SegmentAndVariantsDataProvider))]
        public void GetTableInformation_EntityWithVariants(string country, string commodity, string portfolio, string[] ignore = null)
        {
            var segments = new string[] { country, commodity, portfolio };
            var expected = MockTableInfo<TestModelWithVariants>(segments, ignore);
            var entity = new TestModelWithVariants
            {
                Country = country,
                Commodity = commodity,
                Portfolio = portfolio
            };

            var result = MultiTableParserV2.GetTableInformation(entity);

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


        [Theory]
        [ClassData(typeof(SegmentDataProvider))]
        public void GetTableInformation_SegmentsNoVariants(string country, string commodity, string portfolio)
        {
            var segments = new string[] { country, commodity, portfolio };
            var expected = MockTableInfo<TestModel>(segments);

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


        [Theory]
        [ClassData(typeof(SegmentDataProvider))]
        public void GetTableInformation_EntityNoVariants(string country, string commodity, string portfolio)
        {
            var segments = new string[] { country, commodity, portfolio };
            var expected = MockTableInfo<TestModel>(segments);
            var entity = new TestModel
            {
                Country = country,
                Commodity = commodity,
                Portfolio = portfolio
            };

            var result = MultiTableParserV2.GetTableInformation(entity);

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

        private ITableInfo MockTableInfo<T>(string[] segments, string[] ignoredColumns = null)
        {
            var type = typeof(T);

            var allProps = type.GetProperties().ToList();
            allProps.RemoveAll(p => ignoredColumns?.Contains(p.Name) ?? false);

            var wriProps = allProps.ToList();
            wriProps.RemoveAll(p => p.Name == "Id" || p.Name == "Country" || p.Name == "Commodity" || p.Name == "Portfolio");

            return new TestTableInfo
            {
                EntityType = typeof(T),
                AllProperties = allProps,
                WritableProperties = wriProps,
                KeyProperty = allProps.FirstOrDefault(p => p.Name == "Id"),
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
