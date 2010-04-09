using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents Ordering expression.
    /// </summary>
    public abstract class Order : IColumn
    {
        protected IColumn column;

        /// <summary>
        /// Gets column order applies to.
        /// </summary>
        public IColumn Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Gets table order applies to.
        /// </summary>
        public ITable Table
        {
            get { return this.column.Table; }
        }

        /// <summary>
        /// Gets name of column order applies to.
        /// </summary>
        public string Name
        {
            get { return this.column.Name; }
        }

        /// <summary>
        /// Creates ascending ordering for given IColumn.
        /// </summary>
        /// <param name="Column">IColumn to apply ordering to.</param>
        /// <returns>OrderAsc expression.</returns>
        public static OrderAsc ASC(IColumn Column)
        {
            return
                new OrderAsc(Column);
        }

        /// <summary>
        /// Creates descending ordering for given IColumn.
        /// </summary>
        /// <param name="Column">IColumn to apply ordering to.</param>
        /// <returns>OrderDesc expression.</returns>
        public static OrderDesc DESC(IColumn Column)
        {
            return 
                new OrderDesc(Column);
        }
    }

    /// <summary>
    /// Represents Ascending Ordering expression.
    /// </summary>
    public class OrderAsc : Order
    {
        public OrderAsc(IColumn Column)
        {
            this.column = Column;
        }
    }

    /// <summary>
    /// Represents Descending Ordering expression.
    /// </summary>
    public class OrderDesc : Order
    {
        public OrderDesc(IColumn Column)
        {
            this.column = Column;
        }
    }
}
