using System;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents type of expression.
    /// </summary>
    public enum ExpressionType
    {
        Unspecified     = 0,

        And             = 1,
        Or              = 2,

        Equals          = 11,
        NotEquals       = 12,
        Greater         = 13,
        GreaterOrEquals = 14,
        Less            = 15,
        LessOrEquals    = 16,

        Sum             = 21,
        Subs            = 22,
        Multiply        = 23,
        Divide          = 24,

        StartsWith      = 31,
        EndsWith        = 32,
        Contains        = 33,
    }
}
