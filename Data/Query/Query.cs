using System;
using System.Collections.Generic;
using System.Linq;

namespace Definitif.Data.Query
{
    /// <summary>
    /// Represents abstract query.
    /// </summary>
    /// <typeparam name="ModelType">Type of querying model.</typeparam>
    public abstract class Query<ModelType>
        where ModelType : class, IModel, new()
    {
        protected Order orderBy = null;
        protected Group groupBy = null;
        protected Limit limit = new Limit();

        /// <summary>
        /// Draws query to string.
        /// </summary>
        /// <returns>String representation of query.</returns>
        public override string ToString()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws query to string using drawer specified.
        /// </summary>
        /// <param name="drawer">Drawer to draw query.</param>
        /// <returns>String representation of query.</returns>
        public string ToString(Drawer drawer)
        {
            throw new NotImplementedException();
        }
    }
}
