using System;

namespace Definitif.Data.ObjectSql
{
    public abstract partial class Drawer
    {
        // AUTODOC: Drawer.Draw(ITable Table)
        private string Draw(ITable Table)
        {
            if (Table is Table)
                return
                    this.Draw((Table)Table);
            else if (Table is TAlias)
                return
                    this.Draw((TAlias)Table);
            else if (Table is Join.Join)
                return
                    this.Draw((Join.Join)Table);
            else if (Table is Query.Select)
                return String.Format(
                    "( {0} )",
                    this.Draw((Query.Select)Table));
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
        protected virtual string Draw(TAlias Alias)
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
                    this.Draw((Join.InnerJoin)Join);
            else if (Join is Join.RightJoin)
                return
                    this.Draw((Join.RightJoin)Join);
            else if (Join is Join.LeftJoin)
                return
                    this.Draw((Join.LeftJoin)Join);
            else
                return String.Format(
                    "{0} JOIN {1} ON {2}",
                    this.Draw((ITable)Join.First),
                    this.Draw((ITable)Join.Second),
                    this.Draw(
                        new Expression.AND(
                            Join.ON.ToArray())));
        }

        // AUTODOC: Drawer.Draw(Join.InnerJoin Join)
        protected virtual string Draw(Join.InnerJoin Join)
        {
            return String.Format(
                "{0} INNER JOIN {1} ON {2}",
                this.Draw((ITable)Join.First),
                this.Draw((ITable)Join.Second),
                this.Draw(
                    new Expression.AND(
                        Join.ON.ToArray())));
        }

        // AUTODOC: Drawer.Draw(Join.LeftJoin Join)
        protected virtual string Draw(Join.LeftJoin Join)
        {
            return String.Format(
                "{0} LEFT JOIN {1} ON {2}",
                this.Draw((ITable)Join.First),
                this.Draw((ITable)Join.Second),
                this.Draw(
                    new Expression.AND(
                        Join.ON.ToArray())));
        }

        // AUTODOC: Drawer.Draw(Join.RightJoin Join)
        protected virtual string Draw(Join.RightJoin Join)
        {
            return String.Format(
                "{0} RIGHT JOIN {1} ON {2}",
                this.Draw((ITable)Join.First),
                this.Draw((ITable)Join.Second),
                this.Draw(
                    new Expression.AND(
                        Join.ON.ToArray())));
        }
    }
}
