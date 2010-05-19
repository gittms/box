using System;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents type of join.
    /// </summary>
    public enum JoinType
    {
        Unspecified     = 0,

        Inner           = 1,
        Left            = 2,
        Right           = 3,
    }
}
