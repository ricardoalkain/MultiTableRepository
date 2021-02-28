using System;

namespace MultiTableRepository.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class ExclusiveForAttribute : Attribute
    {
        /// <summary>
        /// List of segments that specify table(s) where this column IS present.
        /// </summary>
        public string[] Segments { get; }

        /// <summary>
        /// Mark this property to be used only for a specific set of segments. This is useful when some
        /// column is relevant to certain tables but not for others, like GasType which is used only by
        /// GAS commodity tables. Exclusive columns will be read and/or written by the <seealso cref="MultiRepository{T}"/>
        /// repository only for this specified segment set.
        /// This attribute can be applied multiple times for different sets of segments.
        /// </summary>
        /// <remarks>
        /// Note: Use <see cref="null"/> to specify a wildcard segment. For instance, (null, "GAS") specifies that
        /// for ANY first segment (country) and ONLY second segments (commodity) with a GAS value.
        /// </remarks>
        /// <param name="segments">Comma separated list of segment names.</param>
        public ExclusiveForAttribute(params string[] segments)
        {
            if (segments == null || segments.Length == 0)
            {
                throw new ArgumentNullException($"{nameof(ExclusiveForAttribute)}: At least one segment must be provided for this attribute.");
            }

            for (int i = 0; i < segments.Length; i++)
            {
                segments[i] = segments[i]?.ToUpper();
            }

            Segments = segments;
        }
    }
}
