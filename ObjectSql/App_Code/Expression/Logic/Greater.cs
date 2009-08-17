using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Expression
{
    // AUTODOC: class Greater
    public class Greater : DoubleContainer
    {
        // AUTODOC: Greater(IExpression[] First, IExpression[] Second)
        public Greater(
            IExpression[] First,
            IExpression[] Second)
        {
            if (First.Length != 1 || Second.Length != 1)
            {
                throw new ObjectSqlException(
                    "Expression.Greater should contain single expression in both First and Second containers."
                    );
            }

            this.first = First;
            this.second = Second;
        }
    }
}
