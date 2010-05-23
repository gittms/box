using System;
using Definitif.Data.Queries;

namespace Definitif.Data.Providers.MsSql
{
    public sealed class Drawer : Queries.Drawer
    {
        public override string Identity
        {
            get { return "SELECT @@IDENTITY"; }
        }

        protected override string DrawQuerySelectPaged(Query query)
        {
            // If query has an empty offset, using TOP.
            if (query.limit.Offset == 0)
            {
                return this.DrawQuerySelect(query, "", "TOP " + query.limit.RowCount.ToString() + " ", "");
            }

            string order = query.orderBy != null ? String.Join(", ", this.DrawList<Order>(query.orderBy)) : "(SELECT 0)",
                   selectSuffix = "ROW_NUMBER() OVER( ORDER BY " + order + " ) AS [_RowNum], ",
                   prefix = "WITH _RowCounter AS ( ",
                   suffix = " ) SELECT * FROM _RowCounter WHERE [_RowNum] >= " + query.limit.Offset.ToString() +
                       " AND [_RowNum] < " + (query.limit.Offset + query.limit.RowCount).ToString();

            return this.DrawQuerySelect(query, prefix, selectSuffix, suffix);
        }

        protected override string DrawColumnWithTable(Column column)
        {
            return column.Table.Name + ".[" + column.Name + "]";
        }
        protected override string DrawColumnStandAlone(Column column)
        {
            return "[" + column.Name + "]";
        }
    }
}
