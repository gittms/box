using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Expression
{
    // AUTODOC
    public class Equals : DoubleContainer
    {
        // AUTODOC: Equals(IExpression[] First, IExpression[] Second)
        public Equals(IExpression[] First, IExpression[] Second) : base(First, Second, false) { }
    }
}
