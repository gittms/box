using System;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents query limit.
    /// </summary>
    public class Limit
    {
        private int from = 0,
                    count = 0;

        /// <summary>
        /// Gets or sets initial starting point.
        /// </summary>
        public int From
        {
            get { return this.from; }
            set { this.from = value; }
        }
        /// <summary>
        /// Gets or sets number of rows to limit.
        /// </summary>
        public int Count
        {
            get { return this.count; }
            set { this.count = value; }
        }

        /// <summary>
        /// Creates an empty instance of limit object.
        /// </summary>
        public Limit()
        { }

        /// <summary>
        /// Creates an instance of query limit object by top value.
        /// </summary>
        /// <param name="top">Number of rows to query.</param>
        public Limit(int top)
        {
            this.count = top;
        }

        /// <summary>
        /// Creates an instance of query limit object by starting value and number of rows.
        /// </summary>
        /// <param name="from">Starting value to query.</param>
        /// <param name="count">Number of rows to query.</param>
        public Limit(int from, int count)
        {
            this.from = from;
            this.count = count;
        }

        /// <summary>
        /// Calculates maximum row number.
        /// </summary>
        public int To()
        {
            return this.from + this.count;
        }
    }
}
