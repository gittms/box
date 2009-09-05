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
    }
}
