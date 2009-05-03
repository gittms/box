using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Expression
{
    // AUTODOC: class Equals
    public class Equals : DoubleContainer
    {
        // AUTODOC: Equals(IExpression[] First, IExpression[] Second)
        public Equals(
            IExpression[] First,
            IExpression[] Second)
        {
            if (First.Length != 1)
            {
                throw new ObjectSqlException(
                    "Expression.Equals should contain single expression in First container."
                    );
            }

            this.first = First;
            this.second = Second;
        }
    }
}
