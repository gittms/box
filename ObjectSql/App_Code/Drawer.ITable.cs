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

        // AUTODOC: Drawer.Draw(Table Table)
        protected virtual string Draw(Table Table)
        {
            return
                Table.Name;
        }

        // AUTODOC: Drawer.Draw(CAlias Alias)
        protected virtual string Draw(TableAlias Alias)
        {
            return String.Format(
                "{0} AS {1}",
                this.Draw(Alias.Table), Alias.Name);
        }

        // AUTODOC: Drawer.Draw(Join.Join Join)
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

        // AUTODOC: Drawer.Draw(Join.InnerJoin Join)
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

        // AUTODOC: Drawer.Draw(Join.LeftJoin Join)
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

        // AUTODOC: Drawer.Draw(Join.RightJoin Join)
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
