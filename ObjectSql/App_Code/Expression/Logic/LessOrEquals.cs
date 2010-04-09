using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Expression
{
    // AUTODOC: class LessOrEquals
    public class LessOrEquals : DoubleContainer
    {
        // AUTODOC: LessOrEquals(IExpression[] First, IExpression[] Second)
        public LessOrEquals(
            IExpression[] First,
            IExpression[] Second)
        {
            if (First.Length != 1 || Second.Length != 1)
            {
                throw new ObjectSqlException(
                    "Expression.LessOrEquals should contain single expression in both First and Second containers."
                    );
            }

            this.first = First;
            this.second = Second;
        }
    }
}
