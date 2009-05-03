using System;
using System.Collections;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents Database object.
    /// </summary>
    public abstract class Database : IEnumerable
    {
        protected Dictionary<string, Table> tables = new Dictionary<string, Table>();
        protected string name;
        protected string connectionString;

        /// <summary>
        /// Gets database tables dictionary.
        /// </summary>
        [Obsolete("Database tables can be requested and enumerated through Database[string Table].")]
        public Dictionary<string, Table> Tables
        {
            get { return this.tables; }
        }
        /// <summary>
        /// Gets database name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }
        /// <summary>
        /// Gets connection string.
        /// </summary>
        public string ConnectionString
        {
            get { return this.connectionString; }
        }

        /// <summary>
        /// Initializes database instance by Connection
        /// String.
        /// </summary>
        /// <param name="ConnectionString">Connection String to use for initialization.</param>
        public abstract void Init(string ConnectionString);

        public abstract Drawer Drawer { get; }

        /// <summary>
        /// Gets database table object by name.
        /// </summary>
        /// <param name="Table">Name of table to get.</param>
        /// <returns>Requested table object.</returns>
        public Table this[string Table]
        {
            get
            {
                if (this.tables.ContainsKey(Table))
                {
                    return this.tables[Table];
                }
                else
                {
                    throw new ObjectSqlException(
                        String.Format(
                            "Database '{0}' does not contain definition for table '{1}'.",
                            this.name, Table
                        ));
                }
            }
        }

        /// <summary>
        /// Adds specified Table object to Database.
        /// </summary>
        /// <param name="Table">Table object to add.</param>
        public void Add(Table Table)
        {
            Table.Database = this;
            this.tables.Add(Table.Name, Table);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.tables.Values.GetEnumerator();
        }
    }
}
