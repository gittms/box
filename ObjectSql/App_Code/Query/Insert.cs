using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Query
{
    /// <summary>
    /// Represents INSERT query object.
    /// </summary>
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

        /// <summary>
        /// Creates INSERT query object with table and list of 
        /// insertable values specified.
        /// </summary>
        /// <param name="Into">Table object to insert into.</param>
        /// <param name="Values">Values expression.</param>
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

        /// <summary>
        /// Creates INSERT query object with list of insertable 
        /// values specified.
        /// </summary>
        /// <param name="Values">Values expression.</param>
        public Insert(
            params IExpression[] Values)
            : this(null, Values)
        { }
    }
}
