using System;
using System.Collections;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents table column object.
    /// </summary>
    public class Column : Operators, IColumn
    {
        private string name;
        private ITable table;

        /// <summary>
        /// Gets column name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }
        /// <summary>
        /// Gets or sets parent Table object.
        /// </summary>
        public ITable Table
        {
            get { return this.table; }
            set { this.table = value; }
        }

        public Column(
            string Name)
        {
            this.name = Name;
        }

        /// <summary>
        /// Creates SUM aggregator.
        /// </summary>
        /// <param name="Column">Column to aggregate value of.</param>
        /// <returns>New SUM aggregator</returns>
        public static Aggregator.Aggregator SUM(Column Column)
        {
            return new Aggregator.Sum(Column);
        }

        /// <summary>
        /// Creates MAX aggregator.
        /// </summary>
        /// <param name="Column">Column to aggregate value of.</param>
        /// <returns>New MAX aggregator</returns>
        public static Aggregator.Aggregator MAX(Column Column)
        {
            return new Aggregator.Max(Column);
        }

        /// <summary>
        /// Creates MIN aggregator.
        /// </summary>
        /// <param name="Column">Column to aggregate value of.</param>
        /// <returns>New MIN aggregator</returns>
        public static Aggregator.Aggregator MIN(Column Column)
        {
            return new Aggregator.Min(Column);
        }

        /// <summary>
        /// Creates column alias object.
        /// </summary>
        /// <param name="Name">Name of alias to create.</param>
        /// <returns>New column alias object.</returns>
        public static ColumnAlias Alias(string Name)
        {
            return new ColumnAlias(Name);
        }

        // Operator == is used to link aliases
        // to columns and represent constructions
        // like:
        // Table.[ID] AS [GUID]
        public static ColumnAlias operator ==(Column First, ColumnAlias Second)
        {
            Second.Column = First;
            return Second;
        }

        public static ColumnAlias operator !=(Column First, ColumnAlias Second)
        {
            throw new ArgumentException(
                "Unable to compare Column to ColumnAlias object.");
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
