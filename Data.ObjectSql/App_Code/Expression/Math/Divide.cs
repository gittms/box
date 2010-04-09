using System;
using System.Collections.Generic;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.ObjectSql.Expression
{
    /// <summary>
    /// Represents / expression:
    /// [IExpression] / [IExpression]
    /// </summary>
    public class Divide : SingleContainer
    {
        public Divide(
            params IExpression[] Expressions)
        {
            this.container = Expressions;
        }
    }
}
