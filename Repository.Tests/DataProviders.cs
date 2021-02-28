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

        protected bool ForceNoVariance = false;

        public virtual IEnumerator<object[]> GetEnumerator()
        {
            var s = ForceNoVariance ? "x" : "";

            foreach (var country in Countries)
            {
                foreach (var commodity in Commodities)
                {
                    foreach (var portfolio in Portfolios)
                    {
                        yield return new string[] { s + country, s + commodity, s + portfolio };
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class SegmentDataProviderNoVariants : SegmentDataProvider
    {
        public SegmentDataProviderNoVariants()
        {
            ForceNoVariance = true;
        }
    }
}
