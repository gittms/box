using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Query
{
    /// <summary>
    /// Represents DELETE query object.
    /// </summary>
    public class Delete : IQuery
    {
        private List<IExpression> where
            = new List<IExpression>();
        private ITable from;
        private Limit limit;

        /// <summary>
        /// Gets where clause.
        /// </summary>
        public List<IExpression> WHERE
        {
            get { return this.where; }
        }
        /// <summary>
        /// Gets or sets table to delete from.
        /// </summary>
        public ITable FROM
        {
            get { return this.from; }
            set { this.from = value; }
        }
        /// <summary>
        /// Gets or sets query limit.
        /// </summary>
        public Limit LIMIT
        {
            get { return this.limit; }
            set { this.limit = value; }
        }

        #region Linq-style extensions.
        /// <summary>
        /// Sets table to delete from.
        /// </summary>
        public Delete From(ITable table)
        {
            this.from = table;
            return this;
        }
        /// <summary>
        /// Sets where predicates.
        /// </summary>
        public Delete Where(params IExpression[] expressions)
        {
            this.where = new List<IExpression>(expressions);
            return this;
        }
        /// <summary>
        /// Sets query limit.
        /// </summary>
        public Delete Limit(int start, int count)
        {
            this.limit = new Limit(start, count);
            return this;
        }
        /// <summary>
        /// Sets query limit.
        /// </summary>
        public Delete Top(int top)
        {
            this.limit = new Limit(top);
            return this;
        }
        #endregion

        /// <summary>
        /// Creates DELETE query object.
        /// </summary>
        public Delete()
        { }

        /// <summary>
        /// Creates DELETE query with delete table and where clause specified.
        /// </summary>
        /// <param name="From">Table to delete from.</param>
        /// <param name="Where">Where clause.</param>
        public Delete(ITable From, params IExpression[] Where)
            : this(From)
        {
            foreach (IExpression clause in Where)
            {
                this.where.Add(clause);
            }
        }

        /// <summary>
        /// Creates DELETE query with delete table specified.
        /// </summary>
        /// <param name="From">Table to delete from.</param>
        public Delete(ITable From)
        {
            this.from = From;
        }

        /// <summary>
        /// Creates a copy of DELETE query object.
        /// </summary>
        public Delete Copy()
        {
            Delete copy = new Delete()
                .Where(this.where.ToArray());
            if (this.from != null) copy.FROM = this.from;
            if (this.limit != null) copy.LIMIT = this.limit;

            return copy;
        }

        object ICloneable.Clone()
        {
            return this.Copy();
        }
    }
}
