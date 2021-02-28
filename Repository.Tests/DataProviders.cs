using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Repository.Tests
{
    public class SegmentDataProvider : IEnumerable<object[]>
    {
        private readonly List<string> Countries = new List<string> { "BE", "FR", "DE" };
        private readonly List<string> Commodities = new List<string> { "GAS", "POWER" };
        private readonly List<string> Portfolios = new List<string> { "B2B", "B2C", "GE" };

        public virtual IEnumerator<object[]> GetEnumerator()
        {
            foreach (var country in Countries)
            {
                foreach (var commodity in Commodities)
                {
                    foreach (var portfolio in Portfolios)
                    {
                        yield return new object[] { country, commodity, portfolio };
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class SegmentAndVariantsDataProvider : SegmentDataProvider
    {
        public override IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "be", "power", "b2b", new[] { nameof(TestModelWithVariants.GasType), nameof(TestModelWithVariants.ValueB2B), }};
            yield return new object[] { "be", "power", "b2c", new[] { nameof(TestModelWithVariants.GasType), nameof(TestModelWithVariants.ValueB2B), }};
            yield return new object[] { "be", "power", "ge",  new[] { nameof(TestModelWithVariants.GasType), nameof(TestModelWithVariants.ValueB2B), }};

            yield return new object[] { "be", "gas",   "b2b", null};
            yield return new object[] { "be", "gas",   "b2c", new[] { nameof(TestModelWithVariants.ValueB2B), }};
            yield return new object[] { "be", "gas",   "ge",  new[] { nameof(TestModelWithVariants.ValueB2B), }};

            yield return new object[] { "fr", "power", "b2b", new[] { nameof(TestModelWithVariants.GasType), nameof(TestModelWithVariants.ValueB2B), }};
            yield return new object[] { "fr", "power", "b2c", new[] { nameof(TestModelWithVariants.GasType), nameof(TestModelWithVariants.ValueB2B), }};
            yield return new object[] { "fr", "power", "ge",  new[] { nameof(TestModelWithVariants.GasType), nameof(TestModelWithVariants.ValueB2B), }};

            yield return new object[] { "fr", "gas",   "b2b", null};
            yield return new object[] { "fr", "gas",   "b2c", new[] { nameof(TestModelWithVariants.ValueB2B), }};
            yield return new object[] { "fr", "gas",   "ge",  new[] { nameof(TestModelWithVariants.ValueB2B), }};

            yield return new object[] { "de", "power", "b2b", new[] { nameof(TestModelWithVariants.GasType), nameof(TestModelWithVariants.ValueB2B), }};
            yield return new object[] { "de", "power", "b2c", new[] { nameof(TestModelWithVariants.GasType), nameof(TestModelWithVariants.ValueB2B), }};
            yield return new object[] { "de", "power", "ge",  new[] { nameof(TestModelWithVariants.GasType), nameof(TestModelWithVariants.ValueB2B), }};

            yield return new object[] { "de", "gas",   "b2b", new[] { nameof(TestModelWithVariants.NoGermanGas), }};
            yield return new object[] { "de", "gas",   "b2c", new[] { nameof(TestModelWithVariants.NoGermanGas), nameof(TestModelWithVariants.ValueB2B), }};
            yield return new object[] { "de", "gas",   "ge",  new[] { nameof(TestModelWithVariants.NoGermanGas), nameof(TestModelWithVariants.ValueB2B), }};
        }
    }
}
