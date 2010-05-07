using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents abstract joins creation class.
    /// </summary>
    public abstract class Joins : IJoinable
    {
        /// <summary>
        /// Creates common JOIN expression.
        /// </summary>
        /// <param name="Table">IJoinable object.</param>
        /// <param name="On">Parametrized array of join predicates.</param>
        /// <returns>JOIN expression.</returns>
        public Join.Join JOIN(IJoinable Table, params IExpression[] On)
        {
            return new Join.Join(this, Table, On);
        }

        /// <summary>
        /// Creates INNER JOIN expression.
        /// </summary>
        /// <param name="Table">IJoinable object.</param>
        /// <param name="On">Parametrized array of join predicates.</param>
        /// <returns>INNER JOIN expression.</returns>
        public Join.InnerJoin INNERJOIN(IJoinable Table, params IExpression[] On)
        {
            return new Join.InnerJoin(this, Table, On);
        }

        /// <summary>
        /// Creates RIGHT JOIN expression.
        /// </summary>
        /// <param name="Table">IJoinable object.</param>
        /// <param name="On">Parametrized array of join predicates.</param>
        /// <returns>RIGHT JOIN expression.</returns>
        public Join.RightJoin RIGHTJOIN(IJoinable Table, params IExpression[] On)
        {
            return new Join.RightJoin(this, Table, On);
        }

        /// <summary>
        /// Creates LEFT JOIN expression.
        /// </summary>
        /// <param name="Table">IJoinable object.</param>
        /// <param name="On">Parametrized array of join predicates.</param>
        /// <returns>LEFT JOIN expression.</returns>
        public Join.LeftJoin LEFTJOIN(IJoinable Table, params IExpression[] On)
        {
            return new Join.LeftJoin(this, Table, On);
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
