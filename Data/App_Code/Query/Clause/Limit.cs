using System;

namespace Definitif.Data.Query
{
    /// <summary>
    /// Represents limiting and paging properties.
    /// </summary>
    public class Limit
    {
        /// <summary>
        /// Gets or sets paging offset.
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// Get or sets rows limit.
        /// </summary>
        public int RowCount { get; set; }

        public Limit()
        {
            this.Offset = 0;
            this.RowCount = -1;
        }
    }
}
