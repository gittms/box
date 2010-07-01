using System;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents type of aggregotor.
    /// </summary>
    public enum AggregatorType
    {
        Unspecified     = 0,

        Sum             = 1,
        Max             = 2,
        Min             = 3,
        Count           = 4,

        Length          = 11,

        Distinct        = 21,
        CountDistinct   = 22,
    }
}
