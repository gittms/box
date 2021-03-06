﻿using System;
using System.Collections.Generic;
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
                   suffix = " ) SELECT * FROM _RowCounter WHERE [_RowNum] > " + (query.limit.Offset).ToString() +
                       " AND [_RowNum] <= " + (query.limit.Offset + query.limit.RowCount).ToString();

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
                   suffix = " ) UPDATE _RowCounter SET " + String.Join(", ", values) + " WHERE [_RowNum] > " + query.limit.Offset.ToString() +
                       " AND [_RowNum] <= " + (query.limit.Offset + query.limit.RowCount).ToString();

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
                   suffix = " ) DELETE FROM _RowCounter WHERE [_RowNum] > " + query.limit.Offset.ToString() +
                       " AND [_RowNum] <= " + (query.limit.Offset + query.limit.RowCount).ToString();

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

        protected override string DrawColumnSpecification(Column column)
        {
            string dataType = column.DataType.ToLower();

            // MsSql specific specifications syntax transformation.
            dataType = dataType.Replace("increment", "IDENTITY(1,1)");

            return column.Name + " " + dataType;
        }

        protected override string DrawExpressionFullText(Expression expression)
        {
            // Preprocessing expression into query string.
            string query = (string)expression.Container[1];
            // TODO: For now, clearing brackets as they're not supported!
            query = query.Replace("(", "").Replace(")", "");
            string[] parts = query.Split(
                new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++ )
            {
                string part = parts[i];
                // Encoding " and ', then trimming + and -.
                string queryPart = part.Replace("'", "''").Replace("\"", "\\\"").TrimStart('+', '-');
                queryPart = "\"" + queryPart + "\"";
                // If part starts with -, then adding NOT before clause.
                if (part.StartsWith("-")) queryPart = "NOT " + queryPart;
                parts[i] = queryPart;
            }
            query = String.Join(" AND ", parts);

            // Finally building query.
            IList<Column> columns = (IList<Column>)expression.Container[0];
            if (columns.Count == 1 && columns[0].IsWildcard)
            {
                return "CONTAINS(*, '" + query + "')";
            }
            else
            {
                return "CONTAINS((" + String.Join(", ", this.DrawColumnList(columns)) + "), '" + query + "')";
            }
        }
    }
}
