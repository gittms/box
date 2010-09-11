using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents abstract query.
    /// </summary>
    public abstract class Query
    {
        internal Table modelTable = null;
        protected QueryType type = QueryType.Select;

        internal IList<Column> fields = null;
        internal IList<Join> joins = null;
        internal Expression where = null;
        internal Expression values = null;
        internal IList<Order> orderBy = null;
        internal IList<Column> groupBy = null;
        internal Limit limit = new Limit();
        internal bool distinct = false;

        /// <summary>
        /// Gets query type enumerator value.
        /// </summary>
        public QueryType Type { get { return type; } internal set { type = value; } }

        /// <summary>
        /// Draws query to string using drawer specified.
        /// </summary>
        /// <returns>String representation of query.</returns>
        protected string Draw(Drawer drawer)
        {
    #if !TRACE
            this.Normalize();
            return drawer.Draw(this);
    #else
    Trace.TraceInformation("> Query.Draw()");
    Trace.Indent();
    Stopwatch watch = Stopwatch.StartNew();

            this.Normalize();

    watch.Stop();
    Trace.TraceInformation("Query normalization time: {0}", watch.Elapsed);
    watch = Stopwatch.StartNew();

            string query = drawer.Draw(this);

    watch.Stop();
    Trace.TraceInformation("Query draw time: {0}", watch.Elapsed);
    Trace.TraceInformation("Result: " + query);
    Trace.Unindent();

            return query;
    #endif
        }

        /// <summary>
        /// Draws query to string using drawer specified.
        /// </summary>
        /// <param name="drawer">Drawer to draw query.</param>
        /// <returns>String representation of query.</returns>
        public string ToString(Drawer drawer)
        {
            return this.Draw(drawer);
        }

        /// <summary>
        /// Clones query by creating query of given type.
        /// </summary>
        /// <typeparam name="T">Type of query to clone.</typeparam>
        internal T Clone<T>()
            where T : Query, new()
        {
            T result = new T();
            result.modelTable = modelTable;
            result.Type = type;

            result.fields = fields;
            result.joins = joins;
            result.where = where;
            result.values = values;
            result.orderBy = orderBy;
            result.groupBy = groupBy;
            result.limit = limit;

            return result;
        }

        /// <summary>
        /// Normalizes column object.
        /// </summary>
        /// <param name="column">Column to normalize.</param>
        /// <param name="replacement">Replacement for jiven column.</param>
        /// <param name="joinWith">List of joins.</param>
        /// <returns>True, if column should be replaced, otherwise false.</returns>
        private bool NormalizeColumn(Column column, out Column replacement, List<Column> joinWith)
        {
            if (column is Alias)
            {
                // If aggregator is not for model primary table,
                // ckecking all possible cases and, replacing it with
                // FK or adding to joins list.
                Aggregator aggr = (column as Alias).Aggregator;
                if (aggr.Column.Table != modelTable)
                {
                    if (aggr.Column.IsPrimaryKey)
                    {
                        Column fk = aggr.Column.GetForeignKeyFor(this.modelTable);
                        if ((object)fk != null)
                        {
                            aggr.Column = fk;
                        }
                        else
                        {
                            joinWith.Add(aggr.Column);
                        }
                    }
                    else
                    {
                        joinWith.Add(aggr.Column);
                    }
                }
            }
            else if (column.Table != modelTable)
            {
                // If column is other's table primary key, trying to get foreign
                // key for main model table, otherwise, adding to join list.
                if (column.IsPrimaryKey)
                {
                    Column fk = column.GetForeignKeyFor(this.modelTable);
                    if ((object)fk != null)
                    {
                        replacement = fk;
                        return true;
                    }
                    // Checking many to many joins.
                    // TODO: This should be done somewhere else,
                    // because such case only works when we have
                    // many to many join already specified.
                    else if (this.joins.Count > 0)
                    {
                        foreach (Join join in this.joins)
                        {
                            Column jk = column.GetForeignKeyFor(join.Table),
                                   jpk = this.modelTable.PrimaryKey.GetForeignKeyFor(join.Table);
                            if ((object)jk != null && (object)jpk != null)
                            {
                                replacement = jpk;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        joinWith.Add(column);
                    }
                }
                else
                {
                    joinWith.Add(column);
                }
            }
            replacement = null;
            return false;
        }
        /// <summary>
        /// Normalizes expression object.
        /// </summary>
        /// <param name="expression">Expression to normalize.</param>
        /// <param name="joinWith">List of joins.</param>
        private void NormalizeExpression(Expression expression, List<Column> joinWith)
        {
            Column replacement;
            if (expression.Container.Count == 0 ||
                !(expression.Container[0] is Column)) return;
            Column column = expression.Container[0] as Column;
            if (this.NormalizeColumn(column, out replacement, joinWith))
            {
                expression.Container[0] = replacement;
            }
        }

        /// <summary>
        /// Normalizes query by detecting joins, foreign keys, etc.
        /// </summary>
        public void Normalize()
        {
            List<Column> joinWith = new List<Column>();
            Column replacement;

            // Normalizing query parts.
            if (this.fields != null) for (int i = 0; i < this.fields.Count; i++)
            {
                Column column = this.fields[i];
                if (this.NormalizeColumn(column, out replacement, joinWith))
                {
                    this.fields[i] = replacement;
                }
            }
            if (this.where != null) foreach (Expression expression in this.where)
            {
                this.NormalizeExpression(expression, joinWith);
            }
            if (this.values != null) foreach (Expression expression in this.values)
            {
                this.NormalizeExpression(expression, joinWith);
            }
            if (this.groupBy != null) for (int i = 0; i < this.groupBy.Count; i++)
            {
                Column column = this.groupBy[i];
                if (this.NormalizeColumn(column, out replacement, joinWith))
                {
                    this.groupBy[i] = replacement;
                }
            }
            if (this.orderBy != null) foreach (Order order in this.orderBy)
            {
                Column column = order.Column;
                if (this.NormalizeColumn(column, out replacement, joinWith))
                {
                    order.Column = replacement;
                }
            }

            // Performing joins if needed.
            if (joinWith.Count > 0)
            {
                if (this.joins == null) this.joins = new List<Join>();

                foreach (Column column in joinWith)
                {
                    bool joinRequired = true;
                    foreach (Join join in this.joins)
                    {
                        if (join.Table == column.Table)
                        {
                            joinRequired = false;
                        }
                    }

                    if (joinRequired)
                    {
                        Column pk = column.Table.PrimaryKey;
                        if ((object)pk == null)
                        {
                            throw new InvalidCastException(String.Format(
                                "Table '{0}' does not contain primary key.", column.Table.Name));
                        }
                        Column fk = pk.GetForeignKeyFor(this.modelTable);
                        if ((object)fk == null)
                        {
                            throw new InvalidCastException(String.Format(
                                "Table '{0}' does not contain foreign key for table '{1}'",
                                this.modelTable.Name, column.Table.Name));
                        }

                        this.joins.Add(new Join()
                        {
                            Type = JoinType.Inner,
                            Table = pk.Table,
                            Clause = (fk == pk),
                        });
                    }
                }
            }
        }
    }
}
