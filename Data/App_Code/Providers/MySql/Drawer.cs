using System;
using Definitif.Data.Queries;

namespace Definitif.Data.Providers.MySql
{
    public sealed class Drawer : Queries.Drawer
    {
        public override string Identity
        {
            get { return "SELECT LAST_INSERT_ID()"; }
        }

        private string Draw(Limit limit)
        {
            string result = " LIMIT ";
            if (limit.Offset > 0) result += limit.Offset.ToString() + ", ";
            result += limit.RowCount.ToString();

            return result;
        }

        protected override string DrawQuerySelectPaged(Query query)
        {
            return this.DrawQuerySelect(query, "", "", this.Draw(query.limit), true);
        }

        protected override string DrawQueryUpdatePaged(Query query)
        {
            return this.DrawQueryUpdate(query, "", "", this.Draw(query.limit));
        }

        protected override string DrawQueryDeletePaged(Query query)
        {
            return this.DrawQueryDelete(query, "", "", this.Draw(query.limit));
        }

        protected override string DrawColumnWithTable(Column column)
        {
            return column.Table.Name + ".`" + column.Name + "`";
        }
        protected override string DrawColumnStandAlone(Column column)
        {
            return "`" + column.Name + "`";
        }
        protected override string DrawColumnAsAlias(Column column)
        {
            return column.Table.Name + ".`" + column.Name + "` AS `" +
                column.Table.Name + "." + column.Name + "`";
        }
        protected override string DrawAggregatorLength(Aggregator aggregator)
        {
            return "LEN(" + this.Draw(aggregator.Column) + ")";
        }

        protected override string DrawColumnSpecification(Column column)
        {
            string dataType = column.DataType;

            // MySql specific specifications syntax transformation.
            dataType = dataType.Replace("increment", "AUTO_INCREMENT");

            return column.Name + " " + dataType;
        }
    }
}
