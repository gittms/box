using System;
using System.Collections.Generic;

namespace Definitif.Data
{
    /// <summary>
    /// Represents database table.
    /// </summary>
    public class Table
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
    }
}
