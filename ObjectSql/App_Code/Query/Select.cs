﻿using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Query
{
    /// <summary>
    /// Represents SELECT query object.
    /// </summary>
    public class Select : IQuery, ITable
    {
        private List<IColumn> values
            = new List<IColumn>();
        private List<ITable> from
            = new List<ITable>();
        private List<IExpression> where
            = new List<IExpression>();
        private List<IColumn> order
            = new List<IColumn>();
        private List<IColumn> group
            = new List<IColumn>();
        private Limit limit;

        /// <summary>
        /// Gets table columns dictionary.
        /// </summary>
        public Dictionary<string, Column> Columns
        {
            get
            {
                Dictionary<string, Column> columns =
                    new Dictionary<string, Column>();

                foreach (IColumn column in this.values)
                {
                    if (column is Column)
                        columns.Add(column.Name, column as Column);
                    else if (column is ColumnAlias && (column as ColumnAlias).Column is Column)
                        columns.Add(column.Name, (column as ColumnAlias).Column as Column);
                }

                return columns;
            }
        }

        /// <summary>
        /// Gets Column object by name.
        /// </summary>
        /// <param name="Column">Name of column to get.</param>
        /// <returns>Column object.</returns>
        public Column this[string Column]
        {
            get
            {
                if (this.Columns.ContainsKey(Column))
                {
                    return this.Columns[Column];
                }
                else
                {
                    throw new ObjectSqlException(
                        String.Format(
                            "Query does not contain values selection for column '{0}'.",
                            Column
                        ));
                }
            }
        }

        /// <summary>
        /// Gets list of values to select.
        /// </summary>
        public List<IColumn> VALUES
        {
            get { return this.values; }
        }
        /// <summary>
        /// Gets list of tables to select from.
        /// 
        /// In list can be used:
        ///     <see cref="Table"/>: Generic table object;
        ///     <see cref="TableAlias"/> or <see cref="Table.Alias"/>: Generic table alias;
        ///     <see cref="Join"/>: Joined tables;
        ///     <see cref="Query.Select"/>: Select sub-query.
        /// </summary>
        public List<ITable> FROM
        {
            get { return this.from; }
        }
        /// <summary>
        /// Gets list of where predicates.
        /// </summary>
        public List<IExpression> WHERE
        {
            get { return this.where; }
        }
        /// <summary>
        /// Gets list of order by predicates.
        /// </summary>
        public List<IColumn> ORDERBY
        {
            get { return this.order; }
        }
        /// <summary>
        /// Gets list of group by predicates.
        /// </summary>
        public List<IColumn> GROUPBY
        {
            get { return this.group; }
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
        /// Sets values to select.
        /// </summary>
        public Select Values(params IColumn[] columns)
        {
            this.values = new List<IColumn>(columns);
            return this;
        }
        /// <summary>
        /// Sets tables to select from.
        /// </summary>
        public Select From(params ITable[] tables)
        {
            this.from = new List<ITable>(tables);
            return this;
        }
        /// <summary>
        /// Sets where predicates.
        /// </summary>
        public Select Where(params IExpression[] expressions)
        {
            this.where = new List<IExpression>(expressions);
            return this;
        }
        /// <summary>
        /// Sets order by predicates.
        /// </summary>
        public Select OrderBy(params IColumn[] columns)
        {
            this.order = new List<IColumn>(columns);
            return this;
        }
        /// <summary>
        /// Sets group by predicates.
        /// </summary>
        public Select GroupBy(params IColumn[] columns)
        {
            this.group = new List<IColumn>(columns);
            return this;
        }
        /// <summary>
        /// Sets query limit.
        /// </summary>
        public Select Limit(int start, int count)
        {
            this.limit = new Limit(start, count);
            return this;
        }
        /// <summary>
        /// Sets query limit.
        /// </summary>
        public Select Top(int top)
        {
            this.limit = new Limit(top);
            return this;
        }
        #endregion

        public Select()
        { }

        /// <summary>
        /// Creates SELECT query object with list of values to select.
        /// </summary>
        /// <param name="Values"></param>
        public Select(
            params IColumn[] Values)
        {
            foreach (IColumn value in Values)
            {
                this.values.Add(value);
            }
        }

        /// <summary>
        /// Updates FROM list based on VALUES collection.
        /// </summary>
        internal void UpdateFrom()
        {
            foreach (IColumn column in this.values)
            {
                if (!this.from.Contains(column.Table))
                    this.from.Add(column.Table);
            }
        }

        /// <summary>
        /// Links table alias to SELECT query object.
        /// </summary>
        /// <param name="First">SELECT query object to link alias to.</param>
        /// <param name="Second">Table alias object to link to SELECT query.</param>
        public static TableAlias operator ==(Query.Select First, TableAlias Second)
        {
            Second.Table = First;
            return Second;
        }

        public static TableAlias operator !=(Query.Select First, TableAlias Second)
        {
            throw new ArgumentException(
                "Unable to compare Query.Select to TableAlias object.");
        }

        [Obsolete("Property Query.Select.Name implemented for interface compatibility only.")]
        public string Name
        {
            get { return ""; }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Creates a copy of SELECT query object.
        /// </summary>
        public Select Copy()
        {
            Select copy = new Select()
                .Values(this.values.ToArray())
                .Where(this.where.ToArray())
                .From(this.from.ToArray())
                .OrderBy(this.order.ToArray())
                .GroupBy(this.group.ToArray());
            if (this.limit != null) copy.LIMIT = this.limit;

            return copy;
        }
    }
}
