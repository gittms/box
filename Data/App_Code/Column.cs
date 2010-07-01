using System;
using System.Collections.Generic;
using Definitif.Data.Queries;

namespace Definitif.Data
{
    /// <summary>
    /// Represents table column.
    /// </summary>
    public class Column : Queries.Operators
    {
        protected List<Column> foreignKeys = new List<Column>();

        /// <summary>
        /// Gets column name.
        /// </summary>
        internal string Name { get; set; }
        /// <summary>
        /// Gets column data type.
        /// </summary>
        internal string DataType { get; set; }

        /// <summary>
        /// Gets table column belongs to.
        /// </summary>
        internal Table Table { get; set; }

        protected Column()
        {
            this.IsPrimaryKey = false;
        }

        /// <summary>
        /// Creates new column instance.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <param name="dataType">Data type used by column.</param>
        public Column(string name, string dataType)
        {
            this.Name = name;
            this.DataType = dataType;
            this.IsPrimaryKey = (name == "Id");
            this.IsWildcard = (name == "*");
            this.IsDualWildcard = (name == "**");
        }

        /// <summary>
        /// Gets foreign keys for this column.
        /// </summary>
        public List<Column> ForeignKeys
        {
            get { return foreignKeys; }
        }

        /// <summary>
        /// Returns true if column is primary key (Id) of table.
        /// </summary>
        public bool IsPrimaryKey { get; internal set; }
        /// <summary>
        /// Returns true if column is a wildcard.
        /// </summary>
        public bool IsWildcard { get; internal set; }
        /// <summary>
        /// Returns true if column is a dual wildcard.
        /// </summary>
        public bool IsDualWildcard { get; internal set; }

        /// <summary>
        /// Gets column of given table that links current column.
        /// </summary>
        /// <param name="table">Table to get link with.</param>
        /// <returns>Column linked with current column.</returns>
        public Column GetForeignKeyFor(Table table)
        {
            foreach (Column fk in this.foreignKeys)
            {
                if (fk.Table == table) return fk;
            }
            return null;
        }

        // Operator & joins multiple Order objects into array.
        public static List<Column> operator &(Column left, Column right)
        {
            return new List<Column>()
            {
                left, right,
            };
        }
        public static List<Column> operator &(Column left, IList<Column> right)
        {
            return new List<Column>(right)
            {
                left,
            };
        }

        /// <summary>
        /// Gets ASC column order.
        /// </summary>
        public Order Asc
        {
            get
            {
                return new Order()
                {
                    Column = this,
                };
            }
        }
        /// <summary>
        /// Gets DESC column order.
        /// </summary>
        public Order Desc
        {
            get
            {
                return new Order()
                {
                    Column = this,
                    OrderType = OrderType.Desc,
                };
            }
        }

        /// <summary>
        /// Gets StartsWith expression.
        /// </summary>
        /// <param name="str">String column starts with.</param>
        public Expression StartsWith(string str)
        {
            return new Expression()
            {
                Type = ExpressionType.StartsWith,
                Container = { this, str },
            };
        }
        /// <summary>
        /// Gets NotStartsWith expression.
        /// </summary>
        /// <param name="str">String column not starts with.</param>
        public Expression NotStartsWith(string str)
        {
            return new Expression()
            {
                Type = ExpressionType.NotStartsWith,
                Container = { this, str },
            };
        }
        /// <summary>
        /// Gets EndsWith expression.
        /// </summary>
        /// <param name="str">String column ends with.</param>
        public Expression EndsWith(string str)
        {
            return new Expression()
            {
                Type = ExpressionType.EndsWith,
                Container = { this, str },
            };
        }
        /// <summary>
        /// Gets NotEndsWith expression.
        /// </summary>
        /// <param name="str">String column not ends with.</param>
        public Expression NotEndsWith(string str)
        {
            return new Expression()
            {
                Type = ExpressionType.NotEndsWith,
                Container = { this, str },
            };
        }
        /// <summary>
        /// Gets Contains expression.
        /// </summary>
        /// <param name="str">String column contains.</param>
        public Expression Contains(string str)
        {
            return new Expression()
            {
                Type = ExpressionType.Contains,
                Container = { this, str },
            };
        }
        /// <summary>
        /// Gets NotContains expression.
        /// </summary>
        /// <param name="str">String column not contains.</param>
        public Expression NotContains(string str)
        {
            return new Expression()
            {
                Type = ExpressionType.NotContains,
                Container = { this, str },
            };
        }

        /// <summary>
        /// Gets Length aggregator.
        /// </summary>
        public Aggregator Length
        {
            get
            {
                return new Aggregator()
                {
                    Column = this,
                    Type = AggregatorType.Length,
                };
            }
        }
        /// <summary>
        /// Gets Sum aggregator.
        /// </summary>
        public Aggregator Sum
        {
            get
            {
                return new Aggregator()
                {
                    Column = this,
                    Type = AggregatorType.Sum,
                };
            }
        }
        /// <summary>
        /// Gets Max aggregator.
        /// </summary>
        public Aggregator Max
        {
            get
            {
                return new Aggregator()
                {
                    Column = this,
                    Type = AggregatorType.Max,
                };
            }
        }
        /// <summary>
        /// Gets Min aggregator.
        /// </summary>
        public Aggregator Min
        {
            get
            {
                return new Aggregator()
                {
                    Column = this,
                    Type = AggregatorType.Min,
                };
            }
        }
        /// <summary>
        /// Gets Count aggregator.
        /// </summary>
        public Aggregator Count
        {
            get
            {
                return new Aggregator()
                {
                    Column = this,
                    Type = AggregatorType.Count,
                };
            }
        }
        /// <summary>
        /// Gets Distinct aggregator.
        /// </summary>
        public Aggregator Distinct
        {
            get
            {
                return new Aggregator()
                {
                    Column = this,
                    Type = AggregatorType.Distinct,
                };
            }
        }
        /// <summary>
        /// Gets Count(Distinct) aggregator.
        /// </summary>
        public Aggregator CountDistinct
        {
            get
            {
                return new Aggregator()
                {
                    Column = this,
                    Type = AggregatorType.CountDistinct,
                };
            }
        }
    }
}
