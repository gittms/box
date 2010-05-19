using System;
using System.Collections.Generic;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents abstract query.
    /// </summary>
    public abstract class Query
    {
        internal IList<Column> fields = null;
        internal IList<Join> joins = null;
        internal Expression where = null;
        internal Expression values = null;
        internal IList<Order> orderBy = null;
        internal IList<Column> groupBy = null;
        internal Limit limit = new Limit();

        /// <summary>
        /// Draws query to string using drawer specified.
        /// </summary>
        /// <returns>String representation of query.</returns>
        protected virtual string Draw(Drawer drawer)
        {
            return drawer.Draw(this);
        }

        /// <summary>
        /// Draws query to string using drawer specified.
        /// </summary>
        /// <param name="drawer">Drawer to draw query.</param>
        /// <returns>String representation of query.</returns>
        public string ToString(Drawer drawer)
        {
            return this.Draw(drawer);
        }

        /// <summary>
        /// Clones query by creating query of given type.
        /// </summary>
        /// <typeparam name="T">Type of query to o.</typeparam>
        internal T Clone<T>()
            where T : Query, new()
        {
            T result = new T();

            result.fields = fields;
            result.joins = joins;
            result.where = where;
            result.values = values;
            result.orderBy = orderBy;
            result.groupBy = groupBy;
            result.limit = limit;

            return result;
        }
    }
}
