using System;
using System.Collections.Generic;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.ObjectSql.Expression
{
    /// <summary>
    /// Represents AND expression:
    /// [IExpression] AND [IExpression]
    /// </summary>
    public class AND : SingleContainer
    {
        // AUTODOC
        public AND(params IExpression[] Expressions) : base(Expressions) { }
    }
}
