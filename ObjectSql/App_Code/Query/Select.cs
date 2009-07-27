using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Query
{
    // AUTODOC: class Query.Select()
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

        // AUTODOC: Select.this[string Column]
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
                            "Query does not contain valuse selection for column '{0}'.",
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

        // AUTODOC: constructor Query.Select()
        public Select()
        { }

        // AUTODOC: constructor Query.Select()
        public Select(
            params IColumn[] Values)
        {
            foreach (IColumn value in Values)
            {
                this.values.Add(value);
            }
        }

        // AUTODOC:
        public void UpdateFrom()
        {
            foreach (IColumn column in this.values)
            {
                if (!this.from.Contains(column.Table))
                    this.from.Add(column.Table);
            }
        }

        // AUTODOC: Select.operator ==(Query.Select First, TAlias Second)
        public static TableAlias operator ==(Query.Select First, TableAlias Second)
        {
            Second.Table = First;
            return Second;
        }

        // AUTODOC: Select.operator !=(Query.Select First, TAlias Second)
        public static TableAlias operator !=(Query.Select First, TableAlias Second)
        {
            throw new ArgumentException(
                "Unable to compare Query.Select to TAlias object.");
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
    }
}
