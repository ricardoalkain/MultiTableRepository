using System;
using Dapper.Contrib.Extensions;
using MultiTableRepository.Attributes;

namespace MultiTableRepository.DemoApp
{
    [MultiTable("MULTI_TABLE")]
    public class MyAwesomeModel
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

        [IgnoreFor(null, "POWER")]
        public string GasType { get; set; }

        [IgnoreFor("BE")]
        public string SomeText { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
