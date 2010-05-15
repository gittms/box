using System;
using System.Collections.Generic;

namespace Definitif.Data
{
    /// <summary>
    /// Represents table column.
    /// </summary>
    public class Column : Queries.Operators
    {
        protected List<Column> foreignKeys = new List<Column>();

        /// <summary>
        /// GEts column name.
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// Gets column data type.
        /// </summary>
        public string DataType { get; internal set; }

        /// <summary>
        /// Gets table column belongs to.
        /// </summary>
        public Table Table { get; internal set; }

        /// <summary>
        /// Creates new column instance.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <param name="dataType">Data type used by column.</param>
        public Column(string name, string dataType)
        {
            this.Name = name;
            this.DataType = dataType;
        }

        /// <summary>
        /// Gets foreign keys for this column.
        /// </summary>
        public List<Column> ForeignKeys
        {
            get { return foreignKeys; }
        }
    }
}
