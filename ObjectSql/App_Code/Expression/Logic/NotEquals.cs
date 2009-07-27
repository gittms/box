using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Expression
{
    // AUTODOC: class NotEquals
    public class NotEquals : DoubleContainer
    {
        // AUTODOC: NotEquals(IExpression[] First, IExpression[] Second)
        public NotEquals(
            IExpression[] First,
            IExpression[] Second)
        {
            if (First.Length != 1)
            {
                throw new ObjectSqlException(
                    "Expression.NotEquals should contain single expression in First container."
                    );
            }

            this.first = First;
            this.second = Second;
        }
    }
}
