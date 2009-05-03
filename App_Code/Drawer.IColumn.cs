using System;

namespace Definitif.Data.ObjectSql
{
    public abstract partial class Drawer
    {
        private const string
            SUM = "SUM",
            MAX = "MAX",
            MIN = "MIN";

        // AUTODOC: Drawer.Draw(IColumn Column)
        private string Draw(IColumn Column)
        {
            if (Column is Column)
                return
                    this.Draw((Column)Column);
            else if (Column is Aggregator.Aggregator)
                return
                    this.Draw((Aggregator.Aggregator)Column);
            else if (Column is Alias)
                return
                    this.Draw((CAlias)Column);
            else if (Column is Order)
                return
                    this.Draw((Order)Column);
            else
                return
                    this.Except(Column);
        }

        /// <summary>
        /// Converts column object to string representation.
        /// </summary>
        /// <param name="Column">Column object.</param>
        /// <returns>Column object string representation.</returns>
        protected virtual string Draw(Column Column)
        {
            return String.Format(
                "{0}.[{1}]",
                Column.Table.Name, Column.Name);
        }

        // AUTODOC: Drawer.Draw(Aggregator.Aggregator Aggregator)
        private string Draw(Aggregator.Aggregator Aggregator)
        {
            if (Aggregator is Aggregator.Sum)
                return
                    this.Draw((Aggregator.Sum)Aggregator);
            else if (Aggregator is Aggregator.Min)
                return
                    this.Draw((Aggregator.Min)Aggregator);
            else if (Aggregator is Aggregator.Max)
                return
                    this.Draw((Aggregator.Max)Aggregator);
            else
                return
                    this.Except(Aggregator);
        }

        // AUTODOC: Drawer.Draw(Aggregator.Sum Aggregator)
        protected virtual string Draw(Aggregator.Sum Aggregator)
        {
            return String.Format(
                "{0}( {1} )",
                SUM, this.Draw(Aggregator.Column));
        }

        // AUTODOC: Drawer.Draw(Aggregator.Min Aggregator)
        protected virtual string Draw(Aggregator.Min Aggregator)
        {
            return String.Format(
                "{0}( {1} )",
                MIN, this.Draw(Aggregator.Column));
        }

        // AUTODOC: Drawer.Draw(Aggregator.Max Aggregator)
        protected virtual string Draw(Aggregator.Max Aggregator)
        {
            return String.Format(
                "{0}( {1} )",
                MAX, this.Draw(Aggregator.Column));
        }

        // AUTODOC: Drawer.Draw(CAlias Alias)
        protected virtual string Draw(CAlias Alias)
        {
            return String.Format(
                "{0} AS [{1}]",
                this.Draw(Alias.Column), Alias.Name);
        }

        // AUTODOC: Drawer.Draw(Order Order)
        private string Draw(Order Order)
        {
            if (Order is OrderAsc)
                return
                    this.Draw((OrderAsc)Order);
            else if (Order is OrderDesc)
                return
                    this.Draw((OrderDesc)Order);
            else
                return
                    this.Except(Order);
        }

        // AUTODOC: Drawer.Draw(OrderAsc Order)
        protected virtual string Draw(OrderAsc Order)
        {
            return String.Format(
                "{0} ASC",
                this.Draw(Order.Column));
        }

        // AUTODOC: Drawer.Draw(OrderDesc Order)
        protected virtual string Draw(OrderDesc Order)
        {
            return String.Format(
                "{0} DESC",
                this.Draw(Order.Column));
        }
    }
}
