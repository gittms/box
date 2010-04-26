using System;

namespace Definitif.Data
{
    /// <summary>
    /// Represents interface for implementing ManyToMany relations.
    /// </summary>
    public interface IManyToManyLinkMapper : IMapper
    {
        /// <summary>
        /// Gets the database field name that should be used
        /// when joining object table with link table
        /// based on the type of supplied model.
        /// This is only required in link-model mappers.
        /// </summary>
        /// <param name="model">Model instance to get type from.</param>
        /// <returns>Database field name.</returns>
        string FieldNameJoin(IModel model);

        /// <summary>
        /// Gets the database field name that should be used
        /// for filtering when joining object table with link table
        /// based on the type of supplied model.
        /// This is only required in link-model mappers.
        /// </summary>
        /// <param name="model">Model instance to get type from.</param>
        /// <returns>Database field name.</returns>
        string FieldNameWhere(IModel model);
    }
}
