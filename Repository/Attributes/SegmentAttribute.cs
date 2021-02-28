using System;
using Dapper.Contrib.Extensions;

namespace MultiTableRepository.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class SegmentAttribute : ComputedAttribute
    {
        private readonly int _index;

        /// <summary>
        /// Defines this property as a segment column in the target table.
        /// Segments are used to split data across multiple tables. For example, if an
        /// entity named PROFILE can exist in the context of different countries and
        /// commodities, apply this attribute over the properties that map the columns
        /// Country and Commodity in the table.
        /// </summary>
        /// <remarks>
        /// Note: As segments work as constant value inside the same table, this current version
        /// treats segments as read-only computed columns set for the segement name as value.
        /// </remarks>
        /// <param name="index">Zro-based index specifying the segment position in table's name, i.e.
        /// for "PROFILE_BE_GAS" the Country property should have index 0 and Commodity index 1.</param>
        public SegmentAttribute(int index)
        {
            _index = index;
        }

        /// <summary>
        /// Position where this properties value appears in the table's name.
        /// </summary>
        public int Index => _index;

        //public bool Optional { get; set; } // TODO: Optional segment
    }
}
