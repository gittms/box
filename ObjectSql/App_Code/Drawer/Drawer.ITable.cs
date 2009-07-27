using System;

namespace Definitif.Data.ObjectSql
{
    public abstract partial class Drawer
    {
        /// <summary>
        /// Converts ITable object to string representation.
        /// </summary>
        /// <param name="Table">ITable object.</param>
        /// <returns>ITable object string representation.</returns>
        private string Draw(ITable Table)
        {
            if (Table is Table)
                return
                    this.Draw(Table as Table);
            else if (Table is TableAlias)
                return
                    this.Draw(Table as TableAlias);
            else if (Table is Join.Join)
                return
                    this.Draw(Table as Join.Join);
            else if (Table is Query.Select)
                return String.Format(
                    "( {0} )",
                    this.Draw(Table as Query.Select));
            else
                return
                    this.Except(Table);
        }

        /// <summary>
        /// Converts Table object to string representation.
        /// </summary>
        /// <param name="Table">Table object.</param>
        /// <returns>Table object string representation.</returns>
        protected virtual string Draw(Table Table)
        {
            return
                Table.Name;
        }

        /// <summary>
        /// Converts Table Alias object to string representation.
        /// </summary>
        /// <param name="Table">Table Alias object.</param>
        /// <returns>Table Alias object string representation.</returns>
        protected virtual string Draw(TableAlias Alias)
        {
            return String.Format(
                "{0} AS {1}",
                this.Draw(Alias.Table), Alias.Name);
        }

        /// <summary>
        /// Converts JOIN object to string representation.
        /// </summary>
        /// <param name="Table">JOIN object.</param>
        /// <returns>JOIN object string representation.</returns>
        protected virtual string Draw(Join.Join Join)
        {
            if (Join is Join.InnerJoin)
                return
                    this.Draw(Join as Join.InnerJoin);
            else if (Join is Join.RightJoin)
                return
                    this.Draw(Join as Join.RightJoin);
            else if (Join is Join.LeftJoin)
                return
                    this.Draw(Join as Join.LeftJoin);
            else
                return String.Format(
                    "{0} JOIN {1} ON {2}",
                    this.Draw(Join.First as ITable),
                    this.Draw(Join.Second as ITable),
                    this.Draw(
                        new Expression.AND(
                            Join.ON.ToArray())));
        }

        /// <summary>
        /// Converts INNER JOIN object to string representation.
        /// </summary>
        /// <param name="Table">INNER JOIN object.</param>
        /// <returns>INNER JOIN object string representation.</returns>
        protected virtual string Draw(Join.InnerJoin Join)
        {
            return String.Format(
                "{0} INNER JOIN {1} ON {2}",
                this.Draw(Join.First as ITable),
                this.Draw(Join.Second as ITable),
                this.Draw(
                    new Expression.AND(
                        Join.ON.ToArray())));
        }

        /// <summary>
        /// Converts LEFT JOIN object to string representation.
        /// </summary>
        /// <param name="Table">LEFT JOIN object.</param>
        /// <returns>LEFT JOIN object string representation.</returns>
        protected virtual string Draw(Join.LeftJoin Join)
        {
            return String.Format(
                "{0} LEFT JOIN {1} ON {2}",
                this.Draw(Join.First as ITable),
                this.Draw(Join.Second as ITable),
                this.Draw(
                    new Expression.AND(
                        Join.ON.ToArray())));
        }

        /// <summary>
        /// Converts RIGHT JOIN object to string representation.
        /// </summary>
        /// <param name="Table">RIGHT JOIN object.</param>
        /// <returns>RIGHT JOIN object string representation.</returns>
        protected virtual string Draw(Join.RightJoin Join)
        {
            return String.Format(
                "{0} RIGHT JOIN {1} ON {2}",
                this.Draw(Join.First as ITable),
                this.Draw(Join.Second as ITable),
                this.Draw(
                    new Expression.AND(
                        Join.ON.ToArray())));
        }
    }
}
