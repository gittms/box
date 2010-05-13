using System;
using System.Collections.Generic;

namespace Definitif.Data.Query
{
    /// <summary>
    /// Represents abstract query.
    /// </summary>
    /// <typeparam name="ModelType">Type of querying model.</typeparam>
    public abstract class Query<ModelType> : IQuery
        where ModelType : class, IModel, new()
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
        /// Draws query to string.
        /// </summary>
        /// <returns>String representation of query.</returns>
        public override string ToString()
        {
            // Getting drawer model was initialized with.
            Drawer drawer = (new ModelType().IMapper() as Mapper<ModelType>).Table.Database.Drawer;

            return this.Draw(drawer);
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
    }
}
