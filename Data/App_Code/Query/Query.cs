using System;
using System.Collections.Generic;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents abstract query.
    /// </summary>
    public abstract class Query
    {
        protected Order orderBy = null;
        protected Group groupBy = null;
        protected Limit limit = new Limit();

        /// <summary>
        /// Draws query to string using drawer specified.
        /// </summary>
        /// <returns>String representation of query.</returns>
        protected abstract string Draw(Drawer drawer);

        /// <summary>
        /// Draws query to string using drawer specified.
        /// </summary>
        /// <param name="drawer">Drawer to draw query.</param>
        /// <returns>String representation of query.</returns>
        public string ToString(Drawer drawer)
        {
            return this.Draw(drawer);
        }
    }
}
