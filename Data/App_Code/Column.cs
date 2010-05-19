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

        protected Column() { }

        /// <summary>
        /// Creates new column instance.
        /// </summary>
        /// <param name="name">Name of column.</param>
        /// <param name="dataType">Data type used by column.</param>
        public Column(string name, string dataType)
        {
            this.Name = name;
            this.DataType = dataType;
        }

        /// <summary>
        /// Gets foreign keys for this column.
        /// </summary>
        public List<Column> ForeignKeys
        {
            get { return foreignKeys; }
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
    }
}
