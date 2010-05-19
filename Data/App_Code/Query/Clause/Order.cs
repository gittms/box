using System;
using System.Collections.Generic;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents order clause.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Gets or sets order type.
        /// </summary>
        public OrderType OrderType { get; set; }
        /// <summary>
        /// Gets or sets ordering column.
        /// </summary>
        public Column Column { get; set; }

        public Order()
        {
            this.OrderType = OrderType.Asc;
        }

        // Operator & joins multiple Order objects into array.
        public static List<Order> operator &(Order left, Order right)
        {
            return new List<Order>()
            {
                left, right,
            };
        }
        public static List<Order> operator &(Order left, IList<Order> right)
        {
            return new List<Order>(right)
            {
                left,
            };
        }
    }
}
