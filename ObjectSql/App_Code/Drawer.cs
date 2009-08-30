using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents common abstract SQL queries drawer.
    /// </summary>
    public abstract partial class Drawer
    {
        /// <summary>
        /// Gets identity query.
        /// </summary>
        public abstract string IDENTITY { get; }

        /// <summary>
        /// Converts IQuery object to string representation.
        /// </summary>
        /// <param name="Query">IQuery object.</param>
        /// <returns>IQuery object string representation.</returns>
        public string Draw(IQuery Query)
        {
                 if (Query is Query.Select) return this.Draw(Query as Query.Select);
            else if (Query is Query.Insert) return this.Draw(Query as Query.Insert);
            else if (Query is Query.Update) return this.Draw(Query as Query.Update);
            else if (Query is Query.Delete) return this.Draw(Query as Query.Delete);
            else return this.Except(Query);
        }

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

            // SELECT Table.[Column] FROM Table
            // [WHERE Table.[Column] = Value]
            // [ORDER BY Table.[Column]]
            // [GROUP BY Table.[Column]]
            return "SELECT " + values + " FROM " + from +
                    ((where != "") ? " WHERE " + where : "") +
                    ((order != "") ? " ORDER BY " + order : "") +
                    ((group != "") ? " GROUP BY " + group : "");
        }

        /// <summary>
        /// Converts UPDATE query object to string representation.
        /// </summary>
        /// <param name="Query">UPDATE query object.</param>
        /// <returns>UPDATE query object string representation.</returns>
        protected virtual string Draw(Query.Update Query)
        {
            string
                values = "",
                where = "",
                tables = "";

            values = this.CommaSeparated(Query.VALUES);
            if (Query.TABLES.Count == 0) Query.UpdateTables();
            tables = this.CommaSeparated(Query.TABLES);

            if (Query.WHERE.Count > 0)
            {
                where = this.Draw(new Expression.AND(Query.WHERE.ToArray()));
            }

            // UPDATE Table SET Table.[Column] = Value
            // [WHERE Table.[Column] = Value]
            return "UPDATE " + tables + " SET " + values +
                ((where != "") ? " WHERE " + where : "");
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
                into = "";

            if (Query.INTO as object == null) Query.UpdateInto();
            into = this.Draw(Query.INTO);

            for (int i = 0; i < Query.VALUES.Count; i++)
            {
                if (Query.VALUES[i] is Expression.Equals &&
                    (Query.VALUES[i] as Expression.Equals).FirstContainer[0] is Expression.Object)
                {
                    if (i > 0 && columns != "")
                    {
                        columns += ", ";
                        values += ", ";
                    }
                    columns += this.Draw((Query.VALUES[i] as Expression.Equals).FirstContainer[0] as Expression.Object);
                    IExpression[] second = (Query.VALUES[i] as Expression.Equals).SecondContainer;
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

            // INSERT INTO Table ( Table.[Column] ) VALUES ( Value )
            return "INSERT INTO " + into +
                " ( " + columns + " ) VALUES ( " + values + " )";
        }

        /// <summary>
        /// Converts DELETE query object to string representation.
        /// </summary>
        /// <param name="Query">DELETE query object.</param>
        /// <returns>DELETE query object string representation.</returns>
        protected virtual string Draw(Query.Delete Query)
        {
            string
                from = "",
                where = "";

            from = this.Draw(Query.FROM);

            if (Query.WHERE.Count > 0)
            {
                where = this.Draw(new Expression.AND(Query.WHERE.ToArray()));
            }

            // DELETE FROM Table [WHERE Table.[Column] > 100]
            return "DELETE FROM " + from +
                ((where != "") ? " WHERE " + where : "");
        }

        #region ITable objects drawing implementation.
        /// <summary>
        /// Converts ITable object to string representation.
        /// </summary>
        /// <param name="Table">ITable object.</param>
        /// <returns>ITable object string representation.</returns>
        private string Draw(ITable Table)
        {
                 if (Table is Table) return this.Draw(Table as Table);
            else if (Table is TableAlias) return this.Draw(Table as TableAlias);
            else if (Table is Join.Join) return this.Draw(Table as Join.Join);
            // ( [SUBSELECT QUERY] )
            else if (Table is Query.Select) return "( " + this.Draw(Table as Query.Select) + " )";
            else return this.Except(Table);
        }

        /// <summary>
        /// Converts Table object to string representation.
        /// </summary>
        /// <param name="Table">Table object.</param>
        /// <returns>Table object string representation.</returns>
        protected virtual string Draw(Table Table)
        {
            return Table.Name;
        }

        /// <summary>
        /// Converts Table Alias object to string representation.
        /// </summary>
        /// <param name="Table">Table Alias object.</param>
        /// <returns>Table Alias object string representation.</returns>
        protected virtual string Draw(TableAlias Alias)
        {
            // Table AS Alias
            return this.Draw(Alias.Table as Table) + " AS " + Alias.Name;
        }

        /// <summary>
        /// Converts JOIN object to string representation.
        /// </summary>
        /// <param name="Table">JOIN object.</param>
        /// <returns>JOIN object string representation.</returns>
        protected virtual string Draw(Join.Join Join)
        {
                 if (Join is Join.InnerJoin) return this.Draw(Join as Join.InnerJoin);
            else if (Join is Join.RightJoin) return this.Draw(Join as Join.RightJoin);
            else if (Join is Join.LeftJoin) return this.Draw(Join as Join.LeftJoin);
            // ITable JOIN ITable ON ( ... AND ... )
            else return this.Draw(Join.First as ITable) + 
                " JOIN " + this.Draw(Join.Second as ITable) +
                " ON " + this.Draw(new Expression.AND(Join.ON.ToArray()));
        }

        /// <summary>
        /// Converts INNER JOIN object to string representation.
        /// </summary>
        /// <param name="Table">INNER JOIN object.</param>
        /// <returns>INNER JOIN object string representation.</returns>
        protected virtual string Draw(Join.InnerJoin Join)
        {
            // ITable INNER JOIN ITable ON ( ... AND ... )
            return this.Draw(Join.First as ITable) +
                " INNER JOIN " + this.Draw(Join.Second as ITable) +
                " ON " + this.Draw(new Expression.AND(Join.ON.ToArray()));
        }

        /// <summary>
        /// Converts LEFT JOIN object to string representation.
        /// </summary>
        /// <param name="Table">LEFT JOIN object.</param>
        /// <returns>LEFT JOIN object string representation.</returns>
        protected virtual string Draw(Join.LeftJoin Join)
        {
            // ITable LEFT JOIN ITable ON ( ... AND ... )
            return this.Draw(Join.First as ITable) +
                " LEFT JOIN " + this.Draw(Join.Second as ITable) +
                " ON " + this.Draw(new Expression.AND(Join.ON.ToArray()));
        }

        /// <summary>
        /// Converts RIGHT JOIN object to string representation.
        /// </summary>
        /// <param name="Table">RIGHT JOIN object.</param>
        /// <returns>RIGHT JOIN object string representation.</returns>
        protected virtual string Draw(Join.RightJoin Join)
        {
            // ITable RIGHT JOIN ITable ON ( ... AND ... )
            return this.Draw(Join.First as ITable) +
                " LEFT JOIN " + this.Draw(Join.Second as ITable) +
                " ON " + this.Draw(new Expression.AND(Join.ON.ToArray()));
        } 
        #endregion

        #region IColumn objects drawing implementation.
        protected string
            SUM = "SUM",
            MAX = "MAX",
            MIN = "MIN",
            DATALENGTH = "DATALENGTH";

        /// <summary>
        /// Converts IColumn object to string representation.
        /// </summary>
        /// <param name="Column">IColumn object.</param>
        /// <returns>Column object string representation.</returns>
        private string Draw(IColumn Column)
        {
                 if (Column is Column) return this.Draw(Column as Column);
            else if (Column is Aggregator.Aggregator) return this.Draw(Column as Aggregator.Aggregator);
            else if (Column is Alias) return this.Draw(Column as ColumnAlias);
            else if (Column is Order) return this.Draw(Column as Order);
            else return this.Except(Column);
        }

        /// <summary>
        /// Converts column object to string representation.
        /// </summary>
        /// <param name="Column">Column object.</param>
        /// <returns>Column object string representation.</returns>
        protected virtual string Draw(Column Column)
        {
            if (Column.Name == "**")
            {
                // Table.[Column] AS [Table_Column], ...
                List<IColumn> columns = new List<IColumn>();
                string prefix = Column.Table.Name + ".";

                foreach (Column column in Column.Table.Columns.Values)
                {
                    if (column.Name.StartsWith("*")) continue;
                    columns.Add(column == new ColumnAlias(prefix + column.Name));
                }

                return this.CommaSeparated(columns);
            }
            else
            {
                // Table.[Column] or Table.*
                return Column.Table.Name +
                    ((Column.Name == "*") ? ".*" : ".[" + Column.Name + "]");
            }
        }

        /// <summary>
        /// Converts Aggregator object to string representation.
        /// </summary>
        /// <param name="Aggregator">Aggregator object.</param>
        /// <returns>Aggregator object string representation.</returns>
        private string Draw(Aggregator.Aggregator Aggregator)
        {
                 if (Aggregator is Aggregator.Sum) return this.Draw(Aggregator as Aggregator.Sum);
            else if (Aggregator is Aggregator.Min) return this.Draw(Aggregator as Aggregator.Min);
            else if (Aggregator is Aggregator.Max) return this.Draw(Aggregator as Aggregator.Max);
            else if (Aggregator is Aggregator.DataLength) return this.Draw(Aggregator as Aggregator.DataLength);
            else return this.Except(Aggregator);
        }

        /// <summary>
        /// Converts SUM aggregator object to string representation.
        /// </summary>
        /// <param name="Aggregator">SUM aggregator object.</param>
        /// <returns>SUM aggregator object string representation.</returns>
        protected virtual string Draw(Aggregator.Sum Aggregator)
        {
            // SUM( Table.[Column] )
            return SUM + "( " + this.Draw(Aggregator.Column) + " )";
        }

        /// <summary>
        /// Converts MIN aggregator object to string representation.
        /// </summary>
        /// <param name="Aggregator">MIN aggregator object.</param>
        /// <returns>MIN aggregator object string representation.</returns>
        protected virtual string Draw(Aggregator.Min Aggregator)
        {
            // MIN( Table.[Column] )
            return MIN + "( " + this.Draw(Aggregator.Column) + " )";
        }

        /// <summary>
        /// Converts MAX aggregator object to string representation.
        /// </summary>
        /// <param name="Aggregator">MAX aggregator object.</param>
        /// <returns>MAX aggregator object string representation.</returns>
        protected virtual string Draw(Aggregator.Max Aggregator)
        {
            // MAX( Table.[Column] )
            return MAX + "( " + this.Draw(Aggregator.Column) + " )";
        }

        /// <summary>
        /// Converts DATALENGTH aggregator object to string representation.
        /// </summary>
        /// <param name="Aggregator">DATALENGTH aggregator object.</param>
        /// <returns>DATALENGTH aggregator object string representation.</returns>
        protected virtual string Draw(Aggregator.DataLength Aggregator)
        {
            // DATALENGTH( Table.[Column] )
            return DATALENGTH + "( " + this.Draw(Aggregator.Column) + " )";
        }

        /// <summary>
        /// Converts column alias object to string representation.
        /// </summary>
        /// <param name="Aggregator">Column alias object.</param>
        /// <returns>Column alias object string representation.</returns>
        protected virtual string Draw(ColumnAlias Alias)
        {
            // Table.[Column] AS [Alias]
            return this.Draw(Alias.Column) + " AS [" + Alias.Name + "]";
        }

        /// <summary>
        /// Converts ordering object to string representation.
        /// </summary>
        /// <param name="Aggregator">Ordering object.</param>
        /// <returns>Ordering object string representation.</returns>
        private string Draw(Order Order)
        {
            if (Order is OrderAsc) return this.Draw(Order as OrderAsc);
            else if (Order is OrderDesc) return this.Draw(Order as OrderDesc);
            else return this.Except(Order);
        }

        /// <summary>
        /// Converts ascending ordering object to string representation.
        /// </summary>
        /// <param name="Aggregator">Ascending ordering object.</param>
        /// <returns>Ascending ordering object string representation.</returns>
        protected virtual string Draw(OrderAsc Order)
        {
            // Table.[Column] ASC
            return this.Draw(Order.Column) + " ASC";
        }

        /// <summary>
        /// Converts descending ordering object to string representation.
        /// </summary>
        /// <param name="Aggregator">Descending ordering object.</param>
        /// <returns>Descending ordering object string representation.</returns>
        protected virtual string Draw(OrderDesc Order)
        {
            // Table.[Column] DESC
            return this.Draw(Order.Column) + " DESC";
        } 
        #endregion

        #region IExpression objects drawing implementation.
        protected string
            AND = "AND",
            OR = "OR";

        /// <summary>
        /// Converts IExpression object to string representation.
        /// </summary>
        /// <param name="Expression">IExpression object.</param>
        /// <returns>IExpression object string representation.</returns>
        private string Draw(IExpression Expression)
        {
                 if (Expression is Expression.Object) return this.Draw(Expression as Expression.Object);
            else if (Expression is Expression.AND) return this.Draw(Expression as Expression.AND);
            else if (Expression is Expression.OR) return this.Draw(Expression as Expression.OR);
            else if (Expression is Expression.Equals) return this.Draw(Expression as Expression.Equals);
            else if (Expression is Expression.NotEquals) return this.Draw(Expression as Expression.NotEquals);
            else if (Expression is Expression.Less) return this.Draw(Expression as Expression.Less);
            else if (Expression is Expression.LessOrEquals) return this.Draw(Expression as Expression.LessOrEquals);
            else if (Expression is Expression.Greater) return this.Draw(Expression as Expression.Greater);
            else if (Expression is Expression.GreaterOrEquals) return this.Draw(Expression as Expression.GreaterOrEquals);
            else if (Expression is Expression.Sum) return this.Draw(Expression as Expression.Sum);
            else if (Expression is Expression.Subs) return this.Draw(Expression as Expression.Subs);
            else if (Expression is Expression.Multiply) return this.Draw(Expression as Expression.Multiply);
            else if (Expression is Expression.Divide) return this.Draw(Expression as Expression.Divide);
            else if (Expression is Expression.LIKE) return this.Draw(Expression as Expression.LIKE);
            else if (Expression is Expression.CONTAINS) return this.Draw(Expression as Expression.CONTAINS);
            else return this.Except(Expression);
        }

        /// <summary>
        /// Converts Object expression to string representation.
        /// </summary>
        /// <param name="Expression">Object expression.</param>
        /// <returns>Object expression string representation.</returns>
        protected virtual string Draw(Expression.Object Object)
        {
                 if (Object.Container is IColumn) return this.Draw(Object.Container as IColumn);
            // Replacing ' char to double '', i.e.
            // 'String''s container'.
            else if (Object.Container is string) return "'" + (Object.Container as string).Replace("'", "''") + "'";
            else if (Object.Container is DateTime)
            {
                DateTime time = (DateTime)Object.Container;
                // MinValue for C# is '1 Jan 0001' and
                // for MS SQL Server - '1 Jan 1900'.
                if (time == DateTime.MinValue) time = new DateTime(1900, 1, 1);
                // MaxValue are the same for C# and
                // MS SQL Server, i.e. '31 Dec 9999'.
                return "'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            else
            {
                string result = Object.Container.ToString();
                // If object string representation is empty,
                // we need to return '' instead of empty string.
                if (result == "") result = "''";
                else
                {
                    // Numeric values can be not surrounded with '',
                    // but string ones must, so we will also replace
                    // ' char to double one ''.
                    if (!Regex.IsMatch(result, @"^[\d\.]+$"))
                    {
                        result = "'" + result.Replace("'", "''") + "'";
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Converts AND expression object to string representation.
        /// </summary>
        /// <param name="Expression">AND expression object.</param>
        /// <returns>AND expression string representation.</returns>
        protected virtual string Draw(Expression.AND Expression)
        {
            // ( Table.[Column] > 100 AND Table.[Column] < 200 )
            string result = "";

            for (int i = 0; i < Expression.Container.Length; i++)
            {
                if (i > 0) result += " " + AND + " ";
                result += this.Draw(Expression.Container[i]);
            }

            if (Expression.Container.Length > 1) return "( " + result + " )";
            else return result;
        }

        /// <summary>
        /// Converts OR expression object to string representation.
        /// </summary>
        /// <param name="Expression">OR expression object.</param>
        /// <returns>OR expression string representation.</returns>
        protected virtual string Draw(Expression.OR Expression)
        {
            // ( Table.[Column] > 100 OR Table.[Column] < 200 )
            string result = "";

            for (int i = 0; i < Expression.Container.Length; i++)
            {
                if (i > 0) result += " " + OR + " ";
                result += this.Draw(Expression.Container[i]);
            }

            return "( " + result + " )";
        }

        /// <summary>
        /// Converts = expression object to string representation.
        /// </summary>
        /// <param name="Expression">= expression object.</param>
        /// <returns>= expression string representation.</returns>
        private string Draw(Expression.Equals Expression)
        {
            return this.Draw(Expression, false);
        }

        /// <summary>
        /// Converts = expression object to string representation.
        /// </summary>
        /// <param name="Expression">= expression object.</param>
        /// <param name="UseEquals">If True, = will be used instead of IS.</param>
        /// <returns>= expression string representation.</returns>
        protected virtual string Draw(Expression.Equals Expression, bool UseEquals)
        {
            string result = "";

            if (Expression.SecondContainer.Length == 1 &&
                Expression.SecondContainer[0] != null)
            {
                // This operator can be applied to subselect,
                // for example WHERE IN ( SELECT ... ).
                if (Expression.SecondContainer[0] is Expression.Object &&
                    (Expression.SecondContainer[0] as Expression.Object).Container is Query.Select)
                {
                    // IN ( [SUBSELECT] )
                    result = this.Draw(Expression.FirstContainer[0]) +
                        " IN ( " + this.Draw((Expression.SecondContainer[0]
                            as Expression.Object).Container as Query.Select) + " )";
                }
                else
                {
                    // Table.[Column] = 'Value'
                    result = this.Draw(Expression.FirstContainer[0]) +
                        " = " + this.Draw(Expression.SecondContainer[0]);
                }
            }
            else if (Expression.SecondContainer.Length == 1 &&
                Expression.SecondContainer[0] == null)
            {
                // Table.[Column] IS NULL or
                // Table.[Column] = NULL
                result = this.Draw(Expression.FirstContainer[0]) +
                    (UseEquals ? " = NULL" : " IS NULL");
            }
            else if (Expression.SecondContainer.Length > 1)
            {
                // Table.[Column] IN ( 1, 2, 3 )
                result = this.Draw(Expression.FirstContainer[0]) +
                    " IN ( " + this.CommaSeparated(Expression.SecondContainer) + " )";
            }

            return result;
        }

        /// <summary>
        /// Converts != expression object to string representation.
        /// </summary>
        /// <param name="Expression">!= expression object.</param>
        /// <returns>!= expression string representation.</returns>
        protected virtual string Draw(Expression.NotEquals Expression)
        {
            string result = "";

            if (Expression.SecondContainer.Length == 1 &&
                Expression.SecondContainer[0] != null)
            {
                // This operator can be applied to subselect,
                // for example WHERE NOT IN ( SELECT ... ).
                if (Expression.SecondContainer[0] is Expression.Object &&
                    (Expression.SecondContainer[0] as Expression.Object).Container is Query.Select)
                {
                    // IN ( [SUBSELECT] )
                    result = this.Draw(Expression.FirstContainer[0]) +
                        " NOT IN ( " + this.Draw((Expression.SecondContainer[0]
                            as Expression.Object).Container as Query.Select) + " )";
                }
                else
                {
                    // Table.[Column] <> 'Value'
                    result = this.Draw(Expression.FirstContainer[0]) +
                        " <> " + this.Draw(Expression.SecondContainer[0]);
                }
            }
            else if (Expression.SecondContainer.Length == 1 &&
                Expression.SecondContainer[0] == null)
            {
                // Table.[Column] IS NOT NULL
                result = this.Draw(Expression.FirstContainer[0]) + " IS NOT NULL";
            }
            else if (Expression.SecondContainer.Length > 1)
            {
                // Table.[Column] NOT IN ( 1, 2, 3 )
                result = this.Draw(Expression.FirstContainer[0]) +
                    " NOT IN ( " + this.CommaSeparated(Expression.SecondContainer) + " )";
            }

            return result;
        }

        /// <summary>
        /// Converts &lt; expression object to string representation.
        /// </summary>
        /// <param name="Expression">&lt; expression object.</param>
        /// <returns>&lt; expression string representation.</returns>
        protected virtual string Draw(Expression.Less Expression)
        {
            // Table.[Column] < 100
            return this.Draw(Expression.FirstContainer[0]) +
                " < " + this.Draw(Expression.SecondContainer[0]);
        }

        /// <summary>
        /// Converts &lt;= expression object to string representation.
        /// </summary>
        /// <param name="Expression">&lt;= expression object.</param>
        /// <returns>&lt;= expression string representation.</returns>
        protected virtual string Draw(Expression.LessOrEquals Expression)
        {
            // Table.[Column] <= 100
            return this.Draw(Expression.FirstContainer[0]) +
                " <= " + this.Draw(Expression.SecondContainer[0]);
        }

        /// <summary>
        /// Converts &gt; expression object to string representation.
        /// </summary>
        /// <param name="Expression">&gt; expression object.</param>
        /// <returns>&gt; expression string representation.</returns>
        protected virtual string Draw(Expression.Greater Expression)
        {
            // Table.[Column] > 100
            return this.Draw(Expression.FirstContainer[0]) +
                " > " + this.Draw(Expression.SecondContainer[0]);
        }

        /// <summary>
        /// Converts &gt;= expression object to string representation.
        /// </summary>
        /// <param name="Expression">&gt;= expression object.</param>
        /// <returns>&gt;= expression string representation.</returns>
        protected virtual string Draw(Expression.GreaterOrEquals Expression)
        {
            // Table.[Column] >= 100
            return this.Draw(Expression.FirstContainer[0]) +
                " >= " + this.Draw(Expression.SecondContainer[0]);
        }

        /// <summary>
        /// Converts + expression object to string representation.
        /// </summary>
        /// <param name="Expression">+ expression object.</param>
        /// <returns>+ expression string representation.</returns>
        protected virtual string Draw(Expression.Sum Expression)
        {
            string result = "";

            for (int i = 0; i < Expression.Container.Length; i++)
            {
                if (i > 0) result += " + ";
                result += this.Draw(Expression.Container[i]);
            }

            return result;
        }

        /// <summary>
        /// Converts - expression object to string representation.
        /// </summary>
        /// <param name="Expression">- expression object.</param>
        /// <returns>- expression string representation.</returns>
        protected virtual string Draw(Expression.Subs Expression)
        {
            string result = "";

            for (int i = 0; i < Expression.Container.Length; i++)
            {
                if (i > 0) result += " - ";
                result += this.Draw(Expression.Container[i]);
            }

            return result;
        }

        /// <summary>
        /// Converts * expression object to string representation.
        /// </summary>
        /// <param name="Expression">* expression object.</param>
        /// <returns>* expression string representation.</returns>
        protected virtual string Draw(Expression.Multiply Expression)
        {
            string result = "";

            for (int i = 0; i < Expression.Container.Length; i++)
            {
                if (i > 0) result += " * ";
                result += this.Draw(Expression.Container[i]);
            }

            return result;
        }

        /// <summary>
        /// Converts / expression object to string representation.
        /// </summary>
        /// <param name="Expression">/ expression object.</param>
        /// <returns>/ expression string representation.</returns>
        protected virtual string Draw(Expression.Divide Expression)
        {
            string result = "";

            for (int i = 0; i < Expression.Container.Length; i++)
            {
                if (i > 0) result += " / ";
                result += this.Draw(Expression.Container[i]);
            }

            return result;
        }

        /// <summary>
        /// Converts LIKE expression object to string representation.
        /// </summary>
        /// <param name="Expression">LIKE expression object.</param>
        /// <returns>LIKE expression string representation.</returns>
        protected virtual string Draw(Expression.LIKE Expression)
        {
            // Table.[Column] LIKE '%expression%'
            return this.Draw(Expression.Container[0]) +
                " LIKE " + this.Draw(new Expression.Object(Expression.Expression));
        }

        /// <summary>
        /// Converts CONTAINS expression object to string representation.
        /// </summary>
        /// <param name="Expression">CONTAINS expression object.</param>
        /// <returns>CONTAINS expression string representation.</returns>
        protected virtual string Draw(Expression.CONTAINS Expression)
        {
            if (Expression.Container == null || Expression.Container.Length == 0)
            {
                // CONTAINS( *, 'expression' )
                return "CONTAINS( *, " +
                    this.Draw(new Expression.Object(Expression.Expression)) + " )";
            }
            else
            {
                string result = this.CommaSeparated(Expression.Container);

                // CONTAINS( Table.[Column], 'expression' )
                return "CONTAINS( " + result + ", " +
                    this.Draw(new Expression.Object(Expression.Expression)) + " )";
            }
        }
        #endregion

        /// <summary>
        /// Throws exception about non implemented drawer.
        /// </summary>
        /// <param name="Object">Object drawer does not support.</param>
        private string Except(object Object)
        {
            #if DEBUG
            return " [UNKNOWN] ";
            #endif
            throw new NotImplementedException(
                String.Format(
                    "Handling of '{0}' is not implemented in Drawer.Draw().",
                    Object.GetType().ToString()
                ));
        }

        /// <summary>
        /// Converts list of ITable objects to comma-separated list representation.
        /// </summary>
        /// <param name="List">List of ITable objects.</param>
        protected string CommaSeparated(IList<ITable> List)
        {
            string result = "";

            for (int i = 0; i < List.Count; i++)
            {
                if (i > 0) result += ", ";
                result += this.Draw(List[i] as ITable);
            }

            return result;
        }
        /// <summary>
        /// Converts list of IColumn objects to comma-separated list representation.
        /// </summary>
        /// <param name="List">List of IColumn objects.</param>
        protected string CommaSeparated(IList<IColumn> List)
        {
            string result = "";

            for (int i = 0; i < List.Count; i++)
            {
                if (i > 0) result += ", ";
                result += this.Draw(List[i] as IColumn);
            }

            return result;
        }
        /// <summary>
        /// Converts list of IExpression objects to comma-separated list representation.
        /// </summary>
        /// <param name="List">List of IExpression objects.</param>
        protected string CommaSeparated(IList<IExpression> List)
        {
            string result = "";

            for (int i = 0; i < List.Count; i++)
            {
                if (i > 0) result += ", ";
                if (List[i] is Expression.Equals)
                {
                    result += this.Draw(List[i] as Expression.Equals, true);
                }
                else
                {
                    result += this.Draw(List[i] as IExpression);
                }
            }

            return result;
        }
        /// <summary>
        /// Converts array of IExpression objects to comma-separated list representation.
        /// </summary>
        /// <param name="List">Array of IExpression objects.</param>
        protected string CommaSeparated(IExpression[] List)
        {
            string result = "";

            for (int i = 0; i < List.Length; i++)
            {
                if (i > 0) result += ", ";
                if (List[i] is Expression.Equals)
                {
                    result += this.Draw(List[i] as Expression.Equals, true);
                }
                else
                {
                    result += this.Draw(List[i] as IExpression);
                }
            }

            return result;
        }
    }
}
