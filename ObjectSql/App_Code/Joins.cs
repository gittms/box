using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    public abstract class Joins : IJoinable
    {
        // AUTODOC: Table.JOIN(IJoinable Table, params IExpression[] On)
        public Join.Join JOIN(IJoinable Table, params IExpression[] On)
        {
            return
                new Join.Join(this, Table, On);
        }

        // AUTODOC: Table.INNERJOIN(IJoinable Table, params IExpression[] On)
        public Join.InnerJoin INNERJOIN(IJoinable Table, params IExpression[] On)
        {
            return
                new Join.InnerJoin(this, Table, On);
        }

        // AUTODOC: Table.RIGHTJOIN(IJoinable Table, params IExpression[] On)
        public Join.RightJoin RIGHTJOIN(IJoinable Table, params IExpression[] On)
        {
            return
                new Join.RightJoin(this, Table, On);
        }

        // AUTODOC: Table.LEFTJOIN(IJoinable Table, params IExpression[] On)
        public Join.LeftJoin LEFTJOIN(IJoinable Table, params IExpression[] On)
        {
            return
                new Join.LeftJoin(this, Table, On);
        }

        public virtual Column this[string Column]
        {
            get { throw new NotImplementedException(); }
        }
        public virtual Dictionary<string, Column> Columns
        {
            get { throw new NotImplementedException(); }
        }
        public virtual string Name
        {
            get { return ""; }
        }
    }
}
