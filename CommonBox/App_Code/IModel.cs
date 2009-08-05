using System;

namespace Definitif.Data.CommonBox
{
    /// <summary>
    /// Defines an entity object with binding to a mapper.
    /// This is a helper interface, not all the methods are defined.
    /// </summary>
    public interface IModel
    {
        Id Id { get; set; }
        int Version { get; set; }
        IMapper Mapper { get; }
        void SubscribeId(Id id);
    }
}
