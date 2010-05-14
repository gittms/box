using System;
using System.Collections.Generic;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents abstract generic query.
    /// </summary>
    /// <typeparam name="ModelType">Type of querying model.</typeparam>
    public abstract class Query<ModelType> : Query
        where ModelType : class, IModel, new()
    {
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
    }
}
