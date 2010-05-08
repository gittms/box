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
        private Limit limit;

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
        /// Gets or sets query limit.
        /// </summary>
        public Limit LIMIT
        {
            get { return this.limit; }
            set { this.limit = value; }
        }

        #region Linq-style extensions.
        /// <summary>
        /// Sets values to insert.
        /// </summary>
        public Insert Values(params IExpression[] values)
        {
            this.values = new List<IExpression>(values);
            return this;
        }
        /// <summary>
        /// Sets table to insert into.
        /// </summary>
        public Insert Into(Table table)
        {
            this.into = table;
            return this;
        }
        #endregion

        /// <summary>
        /// Updates INTO table based on VALUES collection.
        /// </summary>
        internal void UpdateInto()
        {
            foreach (Expression.Equals expression in this.values)
            {
                if (expression.FirstContainer[0] is Expression.Object &&
                    (expression.FirstContainer[0] as Expression.Object).Container is Column)
                {
                    ITable table = ((expression.FirstContainer[0] as Expression.Object).Container as Column).Table;
                    if (table is Table) this.into = table as Table;
                }
            }
        }

        /// <summary>
        /// Creates INSERT query object.
        /// </summary>
        public Insert()
        { }

        /// <summary>
        /// Creates INSERT query object with table and list of 
        /// insertable values specified.
        /// </summary>
        /// <param name="Into">Table object to insert into.</param>
        /// <param name="Values">Values expression.</param>
        public Insert(Table Into, params IExpression[] Values)
            : this(Values)
        {
            this.into = Into;
        }

        /// <summary>
        /// Creates INSERT query object with list of insertable 
        /// values specified.
        /// </summary>
        /// <param name="Values">Values expression.</param>
        public Insert(params IExpression[] Values)
        {
            foreach (IExpression value in Values)
            {
                if (value is Expression.Equals)
                {
                    this.values.Add(value);
                }
                else
                {
                    throw new ObjectSqlException(
                        "Query.Insert VALUES should only contain == expressions.");
                }
            }
        }

        /// <summary>
        /// Creates a copy of INSERT query object.
        /// </summary>
        public Insert Copy()
        {
            Insert copy = new Insert()
                .Values(this.values.ToArray());
            if (!this.into.Equals(null)) copy.INTO = this.into;
            if (this.limit != null) copy.LIMIT = this.limit;

            return copy;
        }

        object ICloneable.Clone()
        {
            return this.Copy();
        }
    }
}
