using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    public abstract partial class Drawer
    {
        /// <summary>
        /// Converts SELECT query object to string representation.
        /// </summary>
        /// <param name="Query">SELECT query object.</param>
        /// <returns>SELECT query object string representation.</returns>
        protected virtual string Draw(Query.Select Query)
        {
            string
                values = "",
                from = "",
                where = "",
                order = "",
                group = "";

            values = this.CommaSeparated(Query.VALUES);
            if (Query.FROM.Count == 0) Query.UpdateFrom();
            from = this.CommaSeparated(Query.FROM);

            if (Query.WHERE.Count > 0)
            {
                where = this.Draw(
                    new Expression.AND(
                        Query.WHERE.ToArray()));
            }

            order = this.CommaSeparated(Query.ORDERBY);
            group = this.CommaSeparated(Query.GROUPBY);

            return String.Format(
                "SELECT {0} FROM {1}" +
                    ((where != "") ? " WHERE {2}" : "") +
                    ((order != "") ? " ORDER BY {3}" : "") +
                    ((group != "") ? " GROUP BY {4}" : ""),
                values, from, where, order, group);
        }

        /// <summary>
        /// Converts UPDATE query object to string representation.
        /// </summary>
        /// <param name="Query">UPDATE query object.</param>
        /// <returns>UPDATE query object string representation.</returns>
        protected virtual string Draw(Query.Update Query)
        {
            return "";
        }

        /// <summary>
        /// Converts INSERT query object to string representation.
        /// </summary>
        /// <param name="Query">INSERT query object.</param>
        /// <returns>INSERT query object string representation.</returns>
        protected virtual string Draw(Query.Insert Query)
        {
            string
                columns = "",
                values = "",
                table = "";

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

            if (Query.INTO as object != null)
            {
                table += this.Draw(Query.INTO);
            }

            return String.Format(
                "INSERT INTO {0} ( {1} ) VALUES ( {2} )",
                table, columns, values);
        }

        /// <summary>
        /// Converts DELETE query object to string representation.
        /// </summary>
        /// <param name="Query">DELETE query object.</param>
        /// <returns>DELETE query object string representation.</returns>
        protected virtual string Draw(Query.Delete Query)
        {
            return "";
        }

        /// <summary>
        /// Converts IQuery object to string representation.
        /// </summary>
        /// <param name="Query">IQuery object.</param>
        /// <returns>IQuery object string representation.</returns>
        public string Draw(IQuery Query)
        {
            if (Query is Query.Select)
                return
                    this.Draw(Query as Query.Select);
            else if (Query is Query.Update)
                return
                    this.Draw(Query as Query.Update);
            else if (Query is Query.Insert)
                return
                    this.Draw(Query as Query.Insert);
            else if (Query is Query.Delete)
                return
                    this.Draw(Query as Query.Delete);
            else
                return
                    this.Except(Query);
        }
    }
}
