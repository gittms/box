using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Query
{
    // AUTODOC: class Query.Update()
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

        // AUTODOC: constructor Query.Update()
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

        // AUTODOC: constructor Query.Update()
        public Update(
            params IExpression[] Values)
            : this(null, Values)
        { }
    }
}
