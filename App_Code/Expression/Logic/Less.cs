using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Expression
{
    // AUTODOC: class Less
    public class Less : DoubleContainer
    {
        // AUTODOC: Less(IExpression[] First, IExpression[] Second)
        public Less(
            IExpression[] First,
            IExpression[] Second)
        {
            if (First.Length != 1 || Second.Length != 1)
            {
                throw new ObjectSqlException(
                    "Expression.Less should contain single expression in both First and Second containers."
                    );
            }

            this.first = First;
            this.second = Second;
        }
    }
}
