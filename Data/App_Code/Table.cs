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

        /// <summary>
        /// Creates specified Column object in Table.
        /// </summary>
        /// <param name="column">Column object to create.</param>
        public void CreateColumn(Column column)
        {
            this.Database.Execute(this.Database.Drawer.DrawColumnCreate(column));
            this.Add(column);
        }
        /// <summary>
        /// Drops specified Column object from Table.
        /// </summary>
        /// <param name="column">Column object to drop.</param>
        public void DropColumn(Column column)
        {
            this.Database.Execute(this.Database.Drawer.DrawColumnDrop(column));
            this.columns.Remove(column.Name);
        }

        /// <summary>
        /// Created table with default layout.
        /// </summary>
        /// <param name="name">Name of table.</param>
        public static Table Default(string name)
        {
            return new Table(name)
            {
                new Column("Id", "int increment"),
                new Column("Version", "int default 0"),
            };
        }
        /// <summary>
        /// Created table with default layout and given columns.
        /// </summary>
        /// <param name="name">Name of table.</param>
        /// <param name="columns">Columns to add to default layout.</param>
        public static Table Default(string name, params Column[] columns)
        {
            Table table = Table.Default(name);
            foreach (Column column in columns) table.Add(column);

            return table;
        }
    }
}
