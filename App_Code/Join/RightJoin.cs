using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Join
{
    // AUTODOC: class Join.RightJoin
    public class RightJoin : Join
    {
        // AUTODOC: constructor Join.RightJoin
        public RightJoin(IJoinable First, IJoinable Second, params IExpression[] On)
        {
            this.first = First;
            this.second = Second;
            foreach (IExpression expression in On)
            {
                this.on.Add(expression);
            }
        }
    }
}
