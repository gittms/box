﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents abstract query drawer.
    /// </summary>
    public abstract partial class Drawer
    {
        public abstract string Identity { get; }

        /// <summary>
        /// Draws given query to string representation.
        /// </summary>
        /// <param name="query">Query to draw.</param>
        /// <returns>String representation of query.</returns>
        public string Draw(Query query)
        {
            bool paged = query.limit.RowCount > 0;
            switch (query.Type)
            {
                case QueryType.Select:
                    return paged ? 
                        this.DrawQuerySelectPaged(query) :
                        this.DrawQuerySelect(query);
                //case QueryType.Insert:
                //    return this.DrawQueryInsert(query);
                //case QueryType.Update:
                //    return paged ?
                //        this.DrawQueryUpdatePaged(query) :
                //        this.DrawQueryUpdate(query);
                //case QueryType.Delete:
                //    return paged ?
                //        this.DrawQueryDeletePaged(query) :
                //        this.DrawQueryDelete(query);
                default:
                    throw new ArgumentException(String.Format(
                        "Drawer does not support queries of type '{0}'.", query.Type.ToString()));
            }
        }

        /// <param name="query">Query to draw.</param>
        /// <param name="prefix">Query prefix.</param>
        /// <param name="selectTermPrefix">Select term prefix, i.e. SELECT [selectTermPrefix] * ...</param>
        /// <param name="suffix">Query suffix.</param>
        protected virtual string DrawQuerySelect(Query query, 
            string prefix, string selectTermPrefix, string suffix)
        {
            return
                prefix +
                "SELECT " +
                    selectTermPrefix +
                    (query.fields == null ? query.modelTable.Name + ".*" : String.Join(", ", this.DrawColumnList(query.fields))) +
               " FROM " +
                    query.modelTable.Name +
                    (query.joins == null ? "" : " " + String.Join(" ", this.DrawList<Join>(query.joins))) +
                (query.where != null ?
               " WHERE " + this.Draw(query.where) : "") +
                (query.orderBy != null ?
               " ORDER BY " + String.Join(", ", this.DrawList<Order>(query.orderBy)) : "") +
                (query.groupBy != null ?
               " GROUP BY " + String.Join(", ", this.DrawList<Column>(query.groupBy)) : "") +
                suffix;
        }

        protected virtual string DrawQuerySelect(Query query)
        {
            return this.DrawQuerySelect(query, "", "", "");
        }
        protected abstract string DrawQuerySelectPaged(Query query);

        /// <summary>
        /// Draws given object to string representation.
        /// </summary>
        /// <param name="obj">Object to draw.</param>
        /// <returns>String representation of object.</returns>
        protected string Draw(object obj)
        {
            // Handling common object types.
            if (obj is Expression) return this.Draw(obj as Expression);
            else if (obj is Query) return "(" + this.Draw(obj as Query) + ")";
            else if (obj is Aggregator) return this.Draw(obj as Aggregator);
            else if (obj is Column) return this.Draw(obj as Column);
            else if (obj is Join) return this.Draw(obj as Join);
            else if (obj is Order) return this.Draw(obj as Order);

            // Replacing ' char to double '', i.e. 'String''s container'.
            else if (obj is string) return "'" + (obj as string).Replace("'", "''") + "'";
            else if (obj is int) return obj.ToString();
            else if (obj is DateTime)
            {
                DateTime time = (DateTime)obj;
                // MinValue for C# is '1 Jan 0001' and
                // for MS SQL Server - '1 Jan 1900'.
                if (time == DateTime.MinValue) time = new DateTime(1900, 1, 1);
                // MaxValue are the same for C# and
                // MS SQL Server, i.e. '31 Dec 9999'.
                return "'" + time.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            else
            {
                string result = obj.ToString();
                // If object string representation is empty,
                // we need to return '' instead of empty string.
                if (result == "") result = "''";
                else
                {
                    // Numeric values can be not surrounded with '',
                    // but string ones must, so we will also replace
                    // ' char to double one ''.
                    if (!Regex.IsMatch(result, @"^[\d\.\,]+$", RegexOptions.Compiled))
                    {
                        result = "'" + result.Replace("'", "''") + "'";
                    }
                    else result = result.Replace(",", ".");
                }
                return result;
            }
        }

        /// <summary>
        /// Draws given list to string representation.
        /// </summary>
        /// <param name="list">List to draw.</param>
        /// <returns>String representation of list.</returns>
        protected string[] DrawList(IList list)
        {
            string[] result = new string[list.Count];
            for (int ind = 0; ind < result.Length; ind++)
            {
                result[ind] = this.Draw(list[ind]);
            }
            return result;
        }
        /// <typeparam name="Type">Type of list members.</typeparam>
        protected string[] DrawList<Type>(IList<Type> list)
            where Type : class
        {
            string[] result = new string[list.Count];
            for (int ind = 0; ind < result.Length; ind++)
            {
                result[ind] = this.Draw(list[ind] as Type);
            }
            return result;
        }

        protected virtual string Draw(Join join)
        {
            string type;
            switch (join.Type)
            {
                case JoinType.Inner: type = "INNER"; break;
                case JoinType.Right: type = "RIGHT"; break;
                case JoinType.Left: type = "LEFT"; break;
                default:
                    throw new ArgumentException(String.Format(
                        "Drawer does not support joins of type '{0}'.", join.Type.ToString()));
            }

            return type + " JOIN " + join.Table.Name + (join.Clause != null ? " ON " + this.Draw(join.Clause) : "");
        }

        /// <summary>
        /// Draws given expression to string representation.
        /// </summary>
        /// <param name="expression">Expression to draw.</param>
        /// <returns>String representation of expression.</returns>
        public string Draw(Expression expression)
        {
            switch (expression.Type)
            {
                case ExpressionType.And:
                    return this.DrawExpressionAnd(expression);
                case ExpressionType.Or:
                    return this.DrawExpressionOr(expression);
                case ExpressionType.Equals:
                    return this.DrawExpressionEquals(expression);
                case ExpressionType.NotEquals:
                    return this.DrawExpressionNotEquals(expression);
                case ExpressionType.Greater:
                    return this.DrawExpressionGreater(expression);
                case ExpressionType.GreaterOrEquals:
                    return this.DrawExpressionGreaterOrEquals(expression);
                case ExpressionType.Less:
                    return this.DrawExpressionLess(expression);
                case ExpressionType.LessOrEquals:
                    return this.DrawExpressionLessOrEquals(expression);
                case ExpressionType.Sum:
                    return this.DrawExpressionSum(expression);
                case ExpressionType.Subs:
                    return this.DrawExpressionSubs(expression);
                case ExpressionType.Multiply:
                    return this.DrawExpressionMultiply(expression);
                case ExpressionType.Divide:
                    return this.DrawExpressionDivide(expression);
                case ExpressionType.StartsWith:
                    return this.DrawExpressionStartsWith(expression);
                case ExpressionType.EndsWith:
                    return this.DrawExpressionEndsWith(expression);
                case ExpressionType.Contains:
                    return this.DrawExpressionContains(expression);
                default:
                    throw new ArgumentException(String.Format(
                        "Drawer does not support expressions of type '{0}'.", expression.Type.ToString()));
            }
        }

        protected virtual string DrawExpressionAnd(Expression expression)
        {
            return "(" + String.Join(" AND ", this.DrawList(expression.Container)) + ")";
        }
        protected virtual string DrawExpressionOr(Expression expression)
        {
            return "(" + String.Join(" OR ", this.DrawList(expression.Container)) + ")";
        }

        protected virtual string DrawExpressionEquals(Expression expression)
        {
            if (expression.Container[1] == null)
            {
                return this.Draw(expression.Container[0]) + " IS NULL";
            }
            else if (expression.Container[1] is IList)
            {
                return this.Draw(expression.Container[0]) + " IN (" +
                    String.Join(", ", this.DrawList(expression.Container[1] as IList)) + ")";
            }
            else if (expression.Container[1] is Query)
            {
                return this.Draw(expression.Container[0]) + " IN (" +
                    this.Draw(expression.Container[1] as Query) + ")";
            }
            else
            {
                return this.Draw(expression.Container[0] as Column) + " = " +
                    this.Draw(expression.Container[1]);
            }
        }
        protected virtual string DrawExpressionNotEquals(Expression expression)
        {
            if (expression.Container[1] == null)
            {
                return this.Draw(expression.Container[0]) + " IS NOT NULL";
            }
            else if (expression.Container[1] is IList)
            {
                return this.Draw(expression.Container[0]) + " NOT IN (" +
                    String.Join(", ", this.DrawList(expression.Container[1] as IList)) + ")";
            }
            else if (expression.Container[1] is Query)
            {
                return this.Draw(expression.Container[0]) + " NOT IN (" +
                    this.Draw(expression.Container[1] as Query) + ")";
            }
            else
            {
                return this.Draw(expression.Container[0]) + " <> " +
                    this.Draw(expression.Container[1]);
            }
        }

        protected virtual string DrawExpressionGreater(Expression expression)
        {
            return this.Draw(expression.Container[0]) + " > " + this.Draw(expression.Container[1]);
        }
        protected virtual string DrawExpressionGreaterOrEquals(Expression expression)
        {
            return this.Draw(expression.Container[0]) + " >= " + this.Draw(expression.Container[1]);
        }
        protected virtual string DrawExpressionLess(Expression expression)
        {
            return this.Draw(expression.Container[0]) + " < " + this.Draw(expression.Container[1]);
        }
        protected virtual string DrawExpressionLessOrEquals(Expression expression)
        {
            return this.Draw(expression.Container[0]) + " <= " + this.Draw(expression.Container[1]);
        }

        protected virtual string DrawExpressionSum(Expression expression)
        {
            return this.Draw(expression.Container[0]) + " + " + this.Draw(expression.Container[1]);
        }
        protected virtual string DrawExpressionSubs(Expression expression)
        {
            return this.Draw(expression.Container[0]) + " - " + this.Draw(expression.Container[1]);
        }
        protected virtual string DrawExpressionMultiply(Expression expression)
        {
            return this.Draw(expression.Container[0]) + " * " + this.Draw(expression.Container[1]);
        }
        protected virtual string DrawExpressionDivide(Expression expression)
        {
            return this.Draw(expression.Container[0]) + " / " + this.Draw(expression.Container[1]);
        }

        protected virtual string DrawExpressionStartsWith(Expression expression)
        {
            return this.Draw(expression.Container[0] as Column) + " LIKE '" + expression.Container[1].ToString() + "%'";
        }
        protected virtual string DrawExpressionEndsWith(Expression expression)
        {
            return this.Draw(expression.Container[0] as Column) + " LIKE '%" + expression.Container[1].ToString() + "'";
        }
        protected virtual string DrawExpressionContains(Expression expression)
        {
            return this.Draw(expression.Container[0] as Column) + " LIKE '%" + expression.Container[1].ToString() + "%'";
        }

        /// <summary>
        /// Draws given order to string representation.
        /// </summary>
        /// <param name="aggregator">Aggregator to draw.</param>
        /// <returns>String representation of order.</returns>
        public virtual string Draw(Order order)
        {
            return this.Draw(order.Column) + " " + ((order.OrderType == OrderType.Asc) ? "ASC" : "DESC");
        }

        /// <summary>
        /// Draws given aggregator to string representation.
        /// </summary>
        /// <param name="aggregator">Aggregator to draw.</param>
        /// <returns>String representation of aggregotor.</returns>
        public string Draw(Aggregator aggregator)
        {
            switch (aggregator.Type)
            {
                case AggregatorType.Length:
                    return this.DrawAggregatorLength(aggregator);
                case AggregatorType.Min:
                    return this.DrawAggregatorMin(aggregator);
                case AggregatorType.Max:
                    return this.DrawAggregatorMax(aggregator);
                case AggregatorType.Sum:
                    return this.DrawAggregatorSum(aggregator);
                case AggregatorType.Count:
                    return this.DrawAggregatorCount(aggregator);
                default:
                    throw new ArgumentException(String.Format(
                        "Drawer does not support aggregators of type '{0}'.", aggregator.Type.ToString()));
            }
        }
        protected virtual string DrawAggregatorLength(Aggregator aggregator)
        {
            return "DATALENGTH(" + this.Draw(aggregator.Column) + ")";
        }
        protected virtual string DrawAggregatorMin(Aggregator aggregator)
        {
            return "MIN(" + this.Draw(aggregator.Column) + ")";
        }
        protected virtual string DrawAggregatorMax(Aggregator aggregator)
        {
            return "MAX(" + this.Draw(aggregator.Column) + ")";
        }
        protected virtual string DrawAggregatorSum(Aggregator aggregator)
        {
            return "SUM(" + this.Draw(aggregator.Column) + ")";
        }
        protected virtual string DrawAggregatorCount(Aggregator aggregator)
        {
            return "COUNT(" + this.Draw(aggregator.Column) + ")";
        }

        /// <summary>
        /// Draws given alias to string representation.
        /// </summary>
        /// <param name="alias">Alias to draw.</param>
        /// <returns>String representation of Alias</returns>
        public virtual string Draw(Alias alias)
        {
            return this.Draw(alias.Aggregator) + " AS " + this.DrawColumnStandAlone(alias);
        }

        /// <summary>
        /// Draws given column to string representation.
        /// </summary>
        /// <param name="column">Column to draw.</param>
        /// <returns>String representation of column.</returns>
        public string Draw(Column column)
        {
            if (column.Table != null) return this.DrawColumnWithTable(column);
            else return this.DrawColumnStandAlone(column);
        }
        protected abstract string DrawColumnWithTable(Column column);
        protected abstract string DrawColumnStandAlone(Column column);

        /// <summary>
        /// Draws given list to string representation as column list.
        /// </summary>
        /// <param name="list">List to draw.</param>
        /// <returns>String representation of list.</returns>
        protected string[] DrawColumnList(IList<Column> list)
        {
            string[] result = new string[list.Count];
            for (int ind = 0; ind < result.Length; ind++)
            {
                if (list[ind] is Alias)
                {
                    result[ind] = this.Draw(list[ind] as Alias);
                }
                else
                {
                    result[ind] = this.Draw(list[ind] as Column);
                }
            }
            return result;
        }
    }
}
