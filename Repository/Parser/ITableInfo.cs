using System;
using System.Collections.Generic;
using System.Reflection;

namespace MultiTableRepository.Parser
{
    public interface ITableInfo
    {
        /// <summary>
        /// Class configured for this group of tables.
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// Prefix name for this group of tables.
        /// </summary>
        string TablePrefix { get; }

        /// <summary>
        /// Property mapped as primary key for these tables.
        /// </summary>
        PropertyInfo KeyProperty { get; }

        /// <summary>
        /// List of properties with <see cref="SegmentAttribute"/> used to define
        /// in which individual table each entity will be persisted.
        /// </summary>
        IReadOnlyList<PropertyInfo> Segments { get; }

        /// <summary>
        /// If <b>true</b> indicates that this group of tables have different columns
        /// according to each set of segments.
        /// </summary>
        bool HasVariants { get; }

        /// <summary>
        /// Full table name for this entity type and segment.
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// Table suffix for this entity type and segment.
        /// </summary>
        string TableSuffix { get; }

        /// <summary>
        /// List of all available columns for this specific table, including
        /// primary key, segment columns and computed columns.
        /// </summary>
        IReadOnlyList<PropertyInfo> AllColumns { get; }

        /// <summary>
        /// List of available <b>writable</b> columns for this specific table.
        /// </summary>
        IReadOnlyList<PropertyInfo> DataColumns { get; }

        /// <summary>
        /// Text containing the column list part of a SQL SELECT statement.
        /// Example: "col1, col2, col3"
        /// </summary>
        string SqlSelectColumnsText { get; }

        /// <summary>
        /// Text containing the column list part of a SQL INSERT statement.
        /// Example: "(col1, col2, col3)"
        /// </summary>
        string SqlInsertColumnNames { get; }

        /// <summary>
        /// Text containing the VALUES part of a SQL INSERT statement.
        /// Example: "VALUES (@col1, @col2, @col3)"
        /// </summary>
        string SqlInsertColumnValues { get; }

        /// <summary>
        /// Text containing the column list part of a SQL UPDATE statement.
        /// Example: "col1=@col1, col2=@col2, col3=@col3"
        /// </summary>
        string SqlUpdateColumnsText { get; }
    }
}

