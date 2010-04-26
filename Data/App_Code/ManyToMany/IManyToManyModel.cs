using System;

namespace Definitif.Data
{
    /// <summary>
    /// Represents interface for implementing ManyToMany relations.
    /// </summary>
    public interface IManyToManyModel : IModel
    {
        IManyToManyLinkMapper IManyToManyLinkMapper();
    }
}
