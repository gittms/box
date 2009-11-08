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
        private List<ITable> tables
            = new List<ITable>();

        /// <summary>
        /// Gets list of UPDATE values.
        /// </summary>
        public List<IExpression> VALUES
        {
            get { return this.values; }
        }

        /// <summary>
        /// Gets list of tables to update.
        /// </summary>
        public List<ITable> TABLES
        {
            get { return this.tables; }
        }
        /// <summary>
        /// Gets list of where predicates.
        /// </summary>
        public List<IExpression> WHERE
        {
            get { return this.where; }
        }

        #region Linq-style extensions.
        /// <summary>
        /// Sets values to update.
        /// </summary>
        public Update Values(params IExpression[] values)
        {
            this.values = new List<IExpression>(values);
            return this;
        }
        /// <summary>
        /// Sets tables to update.
        /// </summary>
        public Update Tables(params ITable[] tables)
        {
            this.tables = new List<ITable>(tables);
            return this;
        }
        /// <summary>
        /// Sets where predicates.
        /// </summary>
        public Update Where(params IExpression[] expressions)
        {
            this.where = new List<IExpression>(expressions);
            return this;
        }
        #endregion

        /// <summary>
        /// Updates TABLES list based on VALUES collection.
        /// </summary>
        internal void UpdateTables()
        {
            foreach (Expression.Equals expression in this.values)
            {
                if (expression.FirstContainer[0] is Expression.Object &&
                    (expression.FirstContainer[0] as Expression.Object).Container is Column)
                {
                    ITable table = ((expression.FirstContainer[0] as Expression.Object).Container as Column).Table;
                    if (!this.tables.Contains(table))
                        this.tables.Add(table);
                }
            }
        }

        /// <summary>
        /// Creates UPDATE query object with update table and expression specified.
        /// </summary>
        /// <param name="Table">Table object to update.</param>
        /// <param name="Values">Parametrized array of IExpression objects representing new values.</param>
        public Update(ITable Table, params IExpression[] Values)
            : this(Values)
        {
            this.tables.Add(Table);
        }

        /// <summary>
        /// Creates UPDATE query object with update expression specified.
        /// </summary>
        /// <param name="Values">Parametrized array of IExpression objects representing new values.</param>
        public Update(params IExpression[] Values)
        {
            foreach (IExpression value in Values)
            {
                if (value is Expression.Equals)
                {
                    this.values.Add(value as Expression.Equals);
                }
                else
                {
                    throw new ObjectSqlException(
                        "Query.Update VALUES should only contain == expressions.");
                }
            }
        }

        /// <summary>
        /// Creates UPDATE query object.
        /// </summary>
        public Update()
        { }
    }
}
