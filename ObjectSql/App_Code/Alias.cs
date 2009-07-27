using System;
using System.Collections;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents abstract alias.
    /// </summary>
    public abstract class Alias : Joins
    {
        protected string name;

        /// <summary>
        /// Gets alias name.
        /// </summary>
        public override string Name
        {
            get { return this.name; }
        }
    }

    /// <summary>
    /// Represents table alias.
    /// </summary>
    public class TableAlias : Alias, ITable, IJoinable
    {
        private ITable table;

        /// <summary>
        /// Gets or sets table alias refers to.
        /// </summary>
        public ITable Table
        {
            get { return this.table; }
            set { this.table = value; }
        }

        /// <summary>
        /// Gets table columns dictionary.
        /// </summary>
        public override Dictionary<string, Column> Columns
        {
            get { return this.table.Columns; }
        }

        /// <summary>
        /// Gets table column object by name.
        /// </summary>
        /// <param name="Column">Name of column to get.</param>
        /// <returns>Requested column object.</returns>
        public override Column this[string Column]
        {
            get
            {
                return this.table[Column];
            }
        }

        public TableAlias(
            string Name)
        {
            this.name = Name;
        }
    }

    /// <summary>
    /// Represents column alias.
    /// </summary>
    public class ColumnAlias : Alias, IColumn
    {
        private IColumn column;

        /// <summary>
        /// Gets or sets column alias refers to.
        /// </summary>
        public IColumn Column
        {
            get { return this.column; }
            set { this.column = value; }
        }

        /// <summary>
        /// Gets table allias applies to.
        /// </summary>
        public ITable Table
        {
            get { return this.column.Table; }
        }

        public ColumnAlias(
            string Name)
        {
            this.name = Name;
        }
    }
}
