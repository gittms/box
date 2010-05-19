using System;
using System.Collections;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents alias.
    /// </summary>
    public class Alias : Column
    {
        /// <summary>
        /// Gets aggregator under alias.
        /// </summary>
        public Aggregator Aggregator { get; internal set; }

        public Alias(Aggregator aggregator, string name)
        {
            this.Aggregator = aggregator;
            this.Name = name;
        }

        public Alias(string name)
        {
            this.Name = name;
        }
	}
}
