using System;

namespace Definitif.Data.Query
{
    /// <summary>
    /// Represents non-generic query interface.
    /// </summary>
    public interface IQuery
    {
        string ToString(Drawer drawer);
    }
}
