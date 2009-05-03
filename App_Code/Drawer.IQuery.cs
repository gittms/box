using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    public abstract partial class Drawer
    {
        // AUTODOC: Drawer.Draw(Query.Select Query)
        protected virtual string Draw(Query.Select Query)
        {
            string
                values = "",
                from = "",
                where = "",
                order = "",
                group = "";

            for (int i = 0; i < Query.VALUES.Count; i++)
            {
                if (i > 0) values += ", ";
                values += this.Draw(Query.VALUES[i]);
            }

            for (int i = 0; i < Query.FROM.Count; i++)
            {
                if (i > 0) from += ", ";
                from += this.Draw(Query.FROM[i]);
            }

            if (Query.WHERE.Count > 0)
            {
                where += this.Draw(
                    new Expression.AND(
                        Query.WHERE.ToArray()));
            }

            for (int i = 0; i < Query.ORDERBY.Count; i++)
            {
                if (i > 0) order += ", ";
                order += this.Draw(Query.ORDERBY[i]);
            }

            for (int i = 0; i < Query.GROUPBY.Count; i++)
            {
                if (i > 0) group += ", ";
                group += this.Draw(Query.GROUPBY[i]);
            }

            return String.Format(
                "SELECT {0} FROM {1}" +
                    ((where != "") ? " WHERE {2}" : "") +
                    ((order != "") ? " ORDER BY {3}" : "") +
                    ((group != "") ? " GROUP BY {4}" : ""),
                values, from, where, order, group);
        }

        // AUTODOC: Drawer.Draw(Query.Update Query)
        protected virtual string Draw(Query.Update Query)
        {
            return "";
        }

        // AUTODOC: Drawer.Draw(Query.Insert Query)
        protected virtual string Draw(Query.Insert Query)
        {
            string
                columns = "",
                values = "",
                table = "",
                where = "";

            for (int i = 0; i < Query.VALUES.Count; i++)
            {
                if (Query.VALUES[i] is Expression.Equals &&
                    ((Expression.Equals)Query.VALUES[i]).FirstContainer[0] is Expression.Object)
                {
                    if (i > 0 && columns != "")
                    {
                        columns += ", ";
                        values += ", ";
                    }
                    columns += this.Draw((Expression.Object)((Expression.Equals)Query.VALUES[i]).FirstContainer[0]);
                    IExpression[] second = ((Expression.Equals)Query.VALUES[i]).SecondContainer;
                    if (second.Length == 1 && second[0] != null)
                    {
                        values += this.Draw(second[0]);
                    }
                    else if (second.Length == 1 && second[0] == null)
                    {
                        values += "NULL";
                    }
                    else
                    {
                        throw new ObjectSqlException(
                            "Query.Insert VALUES should contain single IExpression in Second container.");
                    }
                }
                else
                {
                    throw new ObjectSqlException(
                        "Query.Insert VALUES should contain Expression.Equals expressions with IColumn in First container.");
                }
            }

            if (((object)Query.INTO) != null)
            {
                table += this.Draw(Query.INTO);
            }

            return String.Format(
                "INSERT INTO {0} ( {1} ) VALUES ( {2} )",
                table, columns, values);
        }

        // AUTODOC: Drawer.Draw(IQuery Query)
        internal string Draw(IQuery Query)
        {
            if (Query is Query.Select)
                return
                    this.Draw((Query.Select)Query);
            else if (Query is Query.Update)
                return
                    this.Draw((Query.Update)Query);
            else if (Query is Query.Insert)
                return
                    this.Draw((Query.Insert)Query);
            else
                return
                    this.Except(Query);
        }
    }
}
