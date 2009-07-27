using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Query
{
    // AUTODOC: class Query.Insert()
    public class Insert : IQuery
    {
        private List<IExpression> values
            = new List<IExpression>();
        private Table into;

        /// <summary>
        /// Gets list of INSERT values.
        /// </summary>
        public List<IExpression> VALUES
        {
            get { return this.values; }
        }

        /// <summary>
        /// Gets or sets table to insert into.
        /// </summary>
        public Table INTO
        {
            get { return this.into; }
            set { this.into = value; }
        }

        // AUTODOC: constructor Query.Insert()
        public Insert(
            Table Into,
            params IExpression[] Values)
        {
            this.into = Into;
            foreach (IExpression value in Values)
            {
                this.values.Add(value);
            }
        }

        // AUTODOC: constructor Query.Insert()
        public Insert(
            params IExpression[] Values)
            : this(null, Values)
        { }
    }
}
