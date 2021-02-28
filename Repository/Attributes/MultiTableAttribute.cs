using System;

namespace MultiTableRepository.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class MultiTableAttribute : Attribute
    {
        private readonly string _tableNamePrefix;

        /// <summary>
        /// Defines this class as a multi-table model to be manage by <see cref="Repositories.MultiRepository{T}"/>.
        /// </summary>
        /// <param name="tableNamePrefix">Prefix used to find the corresponding table without segment names.
        /// For example, for "PROFILE_BE_GAS", provide only "PROFILE".</param>
        public MultiTableAttribute(string tableNamePrefix)
        {
            _tableNamePrefix = tableNamePrefix;
        }

        /// <summary>
        /// Base name used to build this entity's table name.
        /// </summary>
        public string TableNamePrefix => _tableNamePrefix;

        /// <summary>
        /// It's recommended to used computed columns for segments. If this is not the case for this entity,
        /// set this property to true to make the repoistory to write segment names in their respective columns.
        /// </summary>
        public bool WriteSegmentColumns { get; set; }
    }
}
