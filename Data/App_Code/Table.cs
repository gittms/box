using System;
using System.Collections;
using System.Collections.Generic;

namespace Definitif.Data
{
    /// <summary>
    /// Represents database table.
    /// </summary>
    public class Table : IEnumerable
    {
        protected Dictionary<string, Column> columns = new Dictionary<string, Column>();

        /// <summary>
        /// Gets database table is lcoated in.
        /// </summary>
        public Database Database { get; internal set; }
        /// <summary>
        /// Gets table name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Creates an instance of database table.
        /// </summary>
        /// <param name="name">Name of table.</param>
        public Table(string name)
        {
            this.Name = name;

            // Creating meta columns for table.
            this.Add(new Column("*", ""));
            this.Add(new Column("**", ""));
        }

        /// <summary>
        /// Gets column by name.
        /// </summary>
        /// <param name="name">Name to get column.</param>
        /// <returns>Column instance.</returns>
        public Column this[string name]
        {
            get
            {
                Column result;
                if (columns.TryGetValue(name, out result))
                {
                    return result;
                }
                else
                {
                    throw new IndexOutOfRangeException(String.Format(
                        "Table '{0}' does not contain definition for '{1}'.",
                        this.Name, name));
                }
            }
        }

        private Column primaryKey;

        /// <summary>
        /// Gets table's primary key column.
        /// </summary>
        public Column PrimaryKey
        {
            get { return primaryKey; }
        }

        /// <summary>
        /// Adds column instance to table.
        /// </summary>
        /// <param name="column">Column instance to add.</param>
        public void Add(Column column)
        {
            column.Table = this;
            this.columns.Add(column.Name, column);
            if (column.IsPrimaryKey) this.primaryKey = column;
        }

        public IEnumerator GetEnumerator()
        {
            return this.columns.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.columns.Values.GetEnumerator();
        }
    }
}
