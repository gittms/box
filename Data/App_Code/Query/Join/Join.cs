using System;
using System.Collections;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents join.
    /// </summary>
    public class Join
    {
        /// <summary>
        /// Gets or sets join type.
        /// </summary>
        public JoinType Type { get; set; }
        /// <summary>
        /// Gets or sets join table.
        /// </summary>
        public Table Table { get; set; }
        /// <summary>
        /// Gets or sets join clause.
        /// </summary>
        public Expression Clause { get; set; }
	}
}
