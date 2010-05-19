using System;
using System.Collections;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents aggregator.
    /// </summary>
    public class Aggregator : Operators
    {
        /// <summary>
        /// Gets or sets aggregator type.
        /// </summary>
        public AggregatorType Type { get; set; }
        /// <summary>
        /// Gets or sets aggregator column.
        /// </summary>
        public Column Column { get; set; }

        /// <summary>
        /// Gets column-casted alias for aggregator.
        /// </summary>
        /// <param name="alias">Name of alias.</param>
        public Column As(string alias)
        {
            return new Alias(this, alias);
        }
	}
}
