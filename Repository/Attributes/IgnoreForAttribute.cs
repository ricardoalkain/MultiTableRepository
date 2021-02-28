using System;

namespace MultiTableRepository.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class IgnoreForAttribute : Attribute
    {
        /// <summary>
        /// List of segments that specify table(s) where this column IS NOT present.
        /// </summary>
        public string[] Segments { get; }

        /// <summary>
        /// Mark this property to be ignored for this specific set of segments. This is useful when some
        /// column is relevant to certain tables but not for others, like GasType which is never used by
        /// POWER commodity tables. Ignored columns won't be read or written by the <seealso cref="MultiRepository{T}"/>
        /// repository. This attribute can be applied multiple times for different sets of segments.
        /// </summary>
        /// <remarks>
        /// Note: Use <see cref="null"/> to specify a wildcard segment. For instance, (null, "POWER") specifies that
        /// for ANY first segment (country) and ignore if second segment (commodity) has a POWER value.
        /// </remarks>
        /// <param name="segments">Comma separated list of segment names.</param>
        public IgnoreForAttribute(params string[] segments)
        {
            if (segments == null || segments.Length == 0)
            {
                throw new ArgumentNullException($"{nameof(IgnoreForAttribute)}: At least one segment must be provided for this attribute.");
            }

            for (int i = 0; i < segments.Length; i++)
            {
                segments[i] = segments[i]?.ToUpper();
            }

            Segments = segments;
        }
    }
}
