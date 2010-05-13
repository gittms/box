using System;

namespace Definitif.Data
{
    /// <summary>
    /// Represents non-generic interface for model table scheme definition.
    /// </summary>
    public interface IModelTableScheme
    {
        Column Id { get; }
        Column Version { get; }
    }
}
