using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Join
{
    // AUTODOC: class Join.InnerJoin
    public class InnerJoin : Join
    {
        // AUTODOC: constructor Join.InnerJoin
        public InnerJoin(IJoinable First, IJoinable Second, params IExpression[] On)
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
