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
        /// Gets foreign keys for this column.
        /// </summary>
        public List<Column> ForeignKeys
        {
            get { return foreignKeys; }
        }
    }
}
