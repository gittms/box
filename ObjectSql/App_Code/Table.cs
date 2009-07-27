using System;
using System.Collections;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents database table object.
    /// </summary>
    public class Table : Joins, IEnumerable, ITable
    {
        private Dictionary<string, Column> columns = new Dictionary<string, Column>();
        private string name;
        private Database database;

        /// <summary>
        /// Gets table columns dictionary.
        /// </summary>
        [Obsolete("Table columns can be requested and enumerated through Table[string Column].")]
        public override Dictionary<string, Column> Columns
        {
            get { return this.columns; }
        }
        /// <summary>
        /// Gets table name.
        /// </summary>
        public override string Name
        {
            get { return this.name; }
        }
        /// <summary>
        /// Gets or sets parent Database object.
        /// </summary>
        internal Database Database
        {
            get { return this.database; }
            set { this.database = value; }
        }

        public Table(
            string Name)
        {
            this.name = Name;
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
                if (this.columns.ContainsKey(Column))
                {
                    return this.columns[Column];
                }
                else
                {
                    throw new ObjectSqlException(
                        String.Format(
                            "Table '{0}' does not contain definition for column '{1}'.",
                            this.name, Column
                        ));
                }
            }
        }

        // AUTODOC: Select.operator ==(Query.Select First, TAlias Second)
        public static TableAlias operator ==(Table First, TableAlias Second)
        {
            Second.Table = First;
            return Second;
        }

        // AUTODOC: Select.operator !=(Query.Select First, TAlias Second)
        public static TableAlias operator !=(Table First, TableAlias Second)
        {
            throw new ArgumentException(
                "Unable to compare Table to TAlias object.");
        }

        /// <summary>
        /// Adds specified Column object to Table.
        /// </summary>
        /// <param name="Column">Column object to add.</param>
        public void Add(Column Column)
        {
            Column.Table = this;
            this.columns.Add(Column.Name, Column);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.columns.Values.GetEnumerator();
        }

        // AUTODOC: Table.Alias(string Name)
        public static TableAlias Alias(string Name)
        {
            return new
                TableAlias(Name);
        }
    }
}
