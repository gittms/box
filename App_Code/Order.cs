using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    // AUTODOC: class OrderAsc
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

        // AUTODOC: Order.Name
        public string Name
        {
            get { return this.column.Name; }
        }

        // AUTODOC: Order.ASC(IColumn Column)
        public static OrderAsc ASC(IColumn Column)
        {
            return
                new OrderAsc(Column);
        }

        // AUTODOC: Order.DESC(IColumn Column)
        public static OrderDesc DESC(IColumn Column)
        {
            return
                new OrderDesc(Column);
        }
    }

    // AUTODOC: class OrderAsc
    public class OrderAsc : Order
    {
        public OrderAsc(IColumn Column)
        {
            this.column = Column;
        }
    }

    // AUTODOC: class OrderDesc
    public class OrderDesc : Order
    {
        public OrderDesc(IColumn Column)
        {
            this.column = Column;
        }
    }
}
