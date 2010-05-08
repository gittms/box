using System;
using System.Data;

namespace Definitif.Data
{
    /// <summary>
    /// Represents data reader mapper.
    /// </summary>
    public class Reader
    {
        private IDataReader reader;

        public Reader(IDataReader Reader)
        {
            this.reader = Reader;
        }

        /// <summary>
        /// Advances reader object to next record.
        /// </summary>
        public bool Read()
        {
            return this.reader.Read();
        }

        /// <param name="i">Zero-based index of column to get.</param>
        public object this[int i]
        {
            get { return this.reader[i]; }
        }

        /// <param name="name">The name of column to get.</param>
        public object this[string name]
        {
            get { return this.reader[name]; }
        }

        /// <summary>
        /// Closes reader object.
        /// </summary>
        public void Close()
        {
            this.reader.Close();
        }
    }
}