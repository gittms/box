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
            // If select query has an empty offset, using TOP.
            if (query.limit.Offset == 0)
            {
                return this.DrawQuerySelect(query, "", "TOP " + query.limit.RowCount.ToString() + " ", "", true);
            }

            string order = query.orderBy != null ? String.Join(", ", this.DrawList<Order>(query.orderBy)) : "(SELECT 0)",
                   selectSuffix = "ROW_NUMBER() OVER( ORDER BY " + order + " ) AS [_RowNum], ",
                   prefix = "WITH _RowCounter AS ( ",
                   suffix = " ) SELECT * FROM _RowCounter WHERE [_RowNum] >= " + query.limit.Offset.ToString() +
                       " AND [_RowNum] < " + (query.limit.Offset + query.limit.RowCount).ToString();

            return this.DrawQuerySelect(query, prefix, selectSuffix, suffix, false);
        }

        protected override string DrawQueryUpdatePaged(Query query)
        {
            // If update query has an empty offset, using TOP.
            if (query.limit.Offset == 0)
            {
                return this.DrawQueryUpdate(query, "", "TOP " + query.limit.RowCount.ToString() + " ", "");
            }

            Expression valuesExpression = new Expression();
            if (query.values.Type == ExpressionType.And) valuesExpression.Container = query.values.Container;
            else valuesExpression.Container.Add(query.values);

            string[] values = new string[valuesExpression.Container.Count];

            for (int i = 0; i < values.Length; i++)
            {
                Expression expression = valuesExpression.Container[i] as Expression;
                values[i] = this.DrawColumnStandAlone(expression.Container[0] as Column) +
                    " = " + this.Draw(expression.Container[1]);
            }

            string order = query.orderBy != null ? String.Join(", ", this.DrawList<Order>(query.orderBy)) : "(SELECT 0)",
                   selectSuffix = "ROW_NUMBER() OVER( ORDER BY " + order + " ) AS [_RowNum], ",
                   prefix = "WITH _RowCounter AS ( ",
                   suffix = " ) UPDATE _RowCounter SET " + String.Join(", ", values) + " WHERE [_RowNum] >= " + query.limit.Offset.ToString() +
                       " AND [_RowNum] < " + (query.limit.Offset + query.limit.RowCount).ToString();

            return this.DrawQuerySelect(query, prefix, selectSuffix, suffix, false);
        }

        protected override string DrawQueryDeletePaged(Query query)
        {
            // If update query has an empty offset, using TOP.
            if (query.limit.Offset == 0)
            {
                return this.DrawQueryDelete(query, "", "TOP (" + query.limit.RowCount.ToString() + ") ", "");
            }

            string order = query.orderBy != null ? String.Join(", ", this.DrawList<Order>(query.orderBy)) : "(SELECT 0)",
                   selectSuffix = "ROW_NUMBER() OVER( ORDER BY " + order + " ) AS [_RowNum], ",
                   prefix = "WITH _RowCounter AS ( ",
                   suffix = " ) DELETE FROM _RowCounter WHERE [_RowNum] >= " + query.limit.Offset.ToString() +
                       " AND [_RowNum] < " + (query.limit.Offset + query.limit.RowCount).ToString();

            return this.DrawQuerySelect(query, prefix, selectSuffix, suffix, false);
        }

        protected override string DrawColumnWithTable(Column column)
        {
            return column.Table.Name + ".[" + column.Name + "]";
        }
        protected override string DrawColumnStandAlone(Column column)
        {
            return "[" + column.Name + "]";
        }
        protected override string DrawColumnAsAlias(Column column)
        {
            return column.Table.Name + ".[" + column.Name + "] AS [" +
                column.Table.Name + "." + column.Name + "]";
        }
    }
}
