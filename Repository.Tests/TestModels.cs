using System;
using Dapper.Contrib.Extensions;
using MultiTableRepository.Attributes;

namespace Repository.Tests
{
    [MultiTable(ParserTests.TABLE_PREFIX)]
    public class TestModel
    {
        [Key]
        public int Id { get; set; }

        [Segment(1)]
        public string Commodity { get; set; }

        [Segment(0)]
        public string Country { get; set; }

        [Segment(2)]
        public string Portfolio { get; set; }

        public string Name { get; set; }

        public double Value { get; set; }

        public string GasType { get; set; }

        public double Ratio { get; set; }

        public double ValueB2B { get; set; }

        public int NoGermanGas { get; set; }

        public DateTime CreatedOn { get; set; }
    }

    [MultiTable(ParserTests.TABLE_PREFIX)]
    public class TestModelWithVariants
    {
        [Key]
        public int Id { get; set; }

        [Segment(0)]
        public string Country { get; set; }

        [Segment(1)]
        public string Commodity { get; set; }

        [Segment(2)]
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
}
