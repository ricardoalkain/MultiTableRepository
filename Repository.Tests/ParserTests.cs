using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MultiTableRepository.Parser;
using Xunit;

namespace Repository.Tests
{
    public partial class ParserTests
    {
        public const string TABLE_PREFIX = "TABLE";
        public const string SEG_SEP = "_";

        private class TestTableInfo : IMultiTableInfo
        {
            public Type EntityType { get; set; }
            public string TablePrefix { get; set; }
            public string TableSuffix { get; set; }
            public PropertyInfo KeyProperty { get; set; }
            public IEnumerable<PropertyInfo> WritableProperties { get; set; }
            public IEnumerable<PropertyInfo> AllProperties { get; set; }

            public string TableName => $"{TablePrefix}_{TableSuffix}";
            public string KeyColumn => KeyProperty?.Name;
            public IEnumerable<string> Columns { get; set; }
            public IEnumerable<string> WritableCols { get; set; }
            public IEnumerable<string> SegmentColumns { get; set; }
            public IEnumerable<PropertyInfo> SegmentProperties { get; set; }
        }

        [Theory]
        [ClassData(typeof(SegmentAndVariantsDataProvider))]
        public void GetTableInformation_SegmenstsWithVariants(string country, string commodity, string portfolio, string[] ignore = null)
        {
            var segments = new string[] { country, commodity, portfolio };
            var expected = MockTableInfo<TestModelWithVariants>(segments, ignore);

            var result = MultiTableParser.GetTableInfo<TestModelWithVariants>(segments);

            Assert.NotNull(result);
            AssertTableInformation(expected, result);
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

            var result = MultiTableParser.GetTableInfo(entity);

            Assert.NotNull(result);
            AssertTableInformation(expected, result);
        }


        [Theory]
        [ClassData(typeof(SegmentDataProvider))]
        public void GetTableInformation_SegmentsNoVariants(string country, string commodity, string portfolio)
        {
            var segments = new string[] { country, commodity, portfolio };
            var expected = MockTableInfo<TestModel>(segments);

            var result = MultiTableParser.GetTableInfo<TestModel>(segments);

            Assert.NotNull(result);
            AssertTableInformation(expected, result);
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

            var result = MultiTableParser.GetTableInfo(entity);

            Assert.NotNull(result);
            AssertTableInformation(expected, result);
        }


        #region Helper methods

        private void AssertTableInformation(IMultiTableInfo expected, IMultiTableInfo result)
        {
            Assert.Equal(expected.EntityType, result.EntityType);
            Assert.Equal(expected.TableName, result.TableName);
            Assert.Equal(expected.TablePrefix, result.TablePrefix);
            Assert.Equal(expected.TableSuffix, result.TableSuffix);
            Assert.Equal(expected.KeyProperty, result.KeyProperty);
            Assert.Equal(expected.KeyColumn, result.KeyColumn);
            Assert.Equal(expected.AllProperties, result.AllProperties);
            Assert.Equal(expected.WritableProperties, result.WritableProperties);
            Assert.Equal(expected.Columns, result.Columns);
            Assert.Equal(expected.WritableCols, result.WritableCols);
            Assert.Equal(expected.SegmentColumns, result.SegmentColumns);
            Assert.Equal(expected.SegmentProperties, result.SegmentProperties);
        }

        private IMultiTableInfo MockTableInfo<T>(string[] segments, string[] ignoredColumns = null)
        {
            var type = typeof(T);

            var allProps = type.GetProperties().ToList();
            allProps.RemoveAll(p => ignoredColumns?.Contains(p.Name) ?? false);

            var segProps = new List<PropertyInfo>();
            // Respect index order without need to parse attributes here
            segProps.Add(allProps.Find(p => p.Name == "Country"));
            segProps.Add(allProps.Find(p => p.Name == "Commodity"));
            segProps.Add(allProps.Find(p => p.Name == "Portfolio"));

            var wriProps = allProps.ToList();
            wriProps.RemoveAll(p => p.Name == "Id" || p.Name == "Country" || p.Name == "Commodity" || p.Name == "Portfolio");

            return new TestTableInfo
            {
                EntityType = typeof(T),
                AllProperties = allProps,
                WritableProperties = wriProps,
                SegmentProperties = segProps,
                KeyProperty = allProps.FirstOrDefault(p => p.Name == "Id"),
                TablePrefix = TABLE_PREFIX,
                TableSuffix = GetSuffix(segments),
                Columns = allProps.Select(p => p.Name).ToList(),
                WritableCols = wriProps.Select(p => p.Name).ToList(),
                SegmentColumns = segProps.Select(p => p.Name).ToList(),
            };
        }

        private string GetSuffix(params string[] segments)
        {
            return string.Join(SEG_SEP, segments);
        }

        #endregion
    }
}
