using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Expression
{
    // AUTODOC: class More
    public class More : DoubleContainer
    {
        // AUTODOC: More(IExpression[] First, IExpression[] Second)
        public More(
            IExpression[] First,
            IExpression[] Second)
        {
            if (First.Length != 1 || Second.Length != 1)
            {
                throw new ObjectSqlException(
                    "Expression.More should contain single expression in both First and Second containers."
                    );
            }

            this.first = First;
            this.second = Second;
        }
    }
}
