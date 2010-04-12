using System;
using System.Collections.Generic;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.ObjectSql.Expression
{
    /// <summary>
    /// Represents OR expression:
    /// [IExpression] OR [IExpression]
    /// </summary>
    public class OR : SingleContainer
    {
        // AUTODOC
        public OR(params IExpression[] Expressions) : base(Expressions) { }
    }
}
