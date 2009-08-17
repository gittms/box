using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Expression
{
    // AUTODOC: class GreaterOrEquals
    public class GreaterOrEquals : DoubleContainer
    {
        // AUTODOC: GreaterOrEquals(IExpression[] First, IExpression[] Second)
        public GreaterOrEquals(
            IExpression[] First,
            IExpression[] Second)
        {
            if (First.Length != 1 || Second.Length != 1)
            {
                throw new ObjectSqlException(
                    "Expression.GreaterOrEquals should contain single expression in both First and Second containers."
                    );
            }

            this.first = First;
            this.second = Second;
        }
    }
}
