using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Join
{
    // AUTODOC: class Join.LeftJoin
    public class LeftJoin : Join
    {
        // AUTODOC: constructor Join.RightJoin
        public LeftJoin(IJoinable First, IJoinable Second, params IExpression[] On)
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
