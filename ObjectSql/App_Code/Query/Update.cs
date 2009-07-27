using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Query
{
    /// <summary>
    /// Represents UPDATE query object.
    /// </summary>
    public class Update : IQuery
    {
        private List<IExpression> values
            = new List<IExpression>();
        private List<IExpression> where
            = new List<IExpression>();
        private Table table;

        /// <summary>
        /// Gets list of UPDATE values.
        /// </summary>
        public List<IExpression> VALUES
        {
            get { return this.values; }
        }

        /// <summary>
        /// Gets or sets table to update.
        /// </summary>
        public Table TABLE
        {
            get { return this.table; }
            set { this.table = value; }
        }
        /// <summary>
        /// Gets list of where predicates.
        /// </summary>
        public List<IExpression> WHERE
        {
            get { return this.where; }
        }

        /// <summary>
        /// Creates UPDATE query object with update table and expression specified.
        /// </summary>
        /// <param name="Table">Table object to update.</param>
        /// <param name="Values">Parametrized array of IExpression objects representing new values.</param>
        public Update(
            Table Table,
            params IExpression[] Values)
        {
            this.table = Table;
            foreach (IExpression value in Values)
            {
                this.values.Add(value);
            }
        }

        /// <summary>
        /// Creates UPDATE query object with update expression specified.
        /// </summary>
        /// <param name="Values">Parametrized array of IExpression objects representing new values.</param>
        public Update(
            params IExpression[] Values)
            : this(null, Values)
        { }
    }
}
