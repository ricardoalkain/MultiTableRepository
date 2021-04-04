using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleFluentSql
{
    public interface IFluentSqlColumns<T>
    {
        /// <summary>
        /// Clears columns list to start adding only custom columns.
        /// This method has the same effect as calling <see cref="SkipColumn"/> for
        /// each one of the current columns.
        /// </summary>
        T ClearColumns();

        /// <summary>
        /// Defines a expression used to set the value of a column.
        /// If the columsn doesn't exist, it is added.
        /// Exemples: SetColumn("Date", "GETDATE()"), SetColumn("Name", "@myParam"),
        /// SetColumn("Code", "'ABC123'")...
        /// </summary>
        /// <remarks>
        /// Note: SetColumn("Id", null) is the same as SetColumn("Id", "@Id").
        /// </remarks>
        /// <param name="columnName">Name of the columns to be customized.</param>
        /// <param name="sqlExpression">SQL expression used to set the value of this column.
        /// Set this value to <see cref="null"/> to use corresponding parameter.</param>
        T SetColumn(string columnName, string sqlExpression);

        /// <summary>
        /// Removes the specified column from the SQL statement.
        /// Use <see cref="SetColumn"/> to include it again.
        /// </summary>
        /// <param name="columnName">Column to be ignored.</param>
        T SkipColumn(string columnName);

        /// <summary>
        /// Reset all changes made to columns. This command restores all default
        /// columns with their original values and remove any customization.
        /// </summary>
        T ResetColumns();
    }
}
