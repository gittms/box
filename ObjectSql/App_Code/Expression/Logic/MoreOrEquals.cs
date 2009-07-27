using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Expression
{
    // AUTODOC: class MoreOrEquals
    public class MoreOrEquals : DoubleContainer
    {
        // AUTODOC: MoreOrEquals(IExpression[] First, IExpression[] Second)
        public MoreOrEquals(
            IExpression[] First,
            IExpression[] Second)
        {
            if (First.Length != 1 || Second.Length != 1)
            {
                throw new ObjectSqlException(
                    "Expression.MoreOrEquals should contain single expression in both First and Second containers."
                    );
            }

            this.first = First;
            this.second = Second;
        }
    }
}
