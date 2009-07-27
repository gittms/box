using System;

namespace Definitif.Data.ObjectSql
{
    public abstract partial class Drawer
    {
        private const string
            SUM = "SUM",
            MAX = "MAX",
            MIN = "MIN";

        /// <summary>
        /// Converts IColumn object to string representation.
        /// </summary>
        /// <param name="Column">IColumn object.</param>
        /// <returns>Column object string representation.</returns>
        private string Draw(IColumn Column)
        {
            if (Column is Column)
                return
                    this.Draw(Column as Column);
            else if (Column is Aggregator.Aggregator)
                return
                    this.Draw(Column as Aggregator.Aggregator);
            else if (Column is Alias)
                return
                    this.Draw(Column as ColumnAlias);
            else if (Column is Order)
                return
                    this.Draw(Column as Order);
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

        /// <summary>
        /// Converts Aggregator object to string representation.
        /// </summary>
        /// <param name="Aggregator">Aggregator object.</param>
        /// <returns>Aggregator object string representation.</returns>
        private string Draw(Aggregator.Aggregator Aggregator)
        {
            if (Aggregator is Aggregator.Sum)
                return
                    this.Draw(Aggregator as Aggregator.Sum);
            else if (Aggregator is Aggregator.Min)
                return
                    this.Draw(Aggregator as Aggregator.Min);
            else if (Aggregator is Aggregator.Max)
                return
                    this.Draw(Aggregator as Aggregator.Max);
            else
                return
                    this.Except(Aggregator);
        }

        /// <summary>
        /// Converts SUM aggregator object to string representation.
        /// </summary>
        /// <param name="Aggregator">SUM aggregator object.</param>
        /// <returns>SUM aggregator object string representation.</returns>
        protected virtual string Draw(Aggregator.Sum Aggregator)
        {
            return String.Format(
                "{0}( {1} )",
                SUM, this.Draw(Aggregator.Column));
        }

        /// <summary>
        /// Converts MIN aggregator object to string representation.
        /// </summary>
        /// <param name="Aggregator">MIN aggregator object.</param>
        /// <returns>MIN aggregator object string representation.</returns>
        protected virtual string Draw(Aggregator.Min Aggregator)
        {
            return String.Format(
                "{0}( {1} )",
                MIN, this.Draw(Aggregator.Column));
        }

        /// <summary>
        /// Converts MAX aggregator object to string representation.
        /// </summary>
        /// <param name="Aggregator">MAX aggregator object.</param>
        /// <returns>MAX aggregator object string representation.</returns>
        protected virtual string Draw(Aggregator.Max Aggregator)
        {
            return String.Format(
                "{0}( {1} )",
                MAX, this.Draw(Aggregator.Column));
        }

        /// <summary>
        /// Converts column alias object to string representation.
        /// </summary>
        /// <param name="Aggregator">Column alias object.</param>
        /// <returns>Column alias object string representation.</returns>
        protected virtual string Draw(ColumnAlias Alias)
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
                    this.Draw(Order as OrderAsc);
            else if (Order is OrderDesc)
                return
                    this.Draw(Order as OrderDesc);
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
