using System;
using System.Collections.Generic;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents update query.
    /// </summary>
    /// <typeparam name="ModelType">Type of querying model.</typeparam>
    public class Update<ModelType> : Query<ModelType>
        where ModelType : class, IModel, new()
    {
        /// <summary>
        /// Specifies values to update.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Update<ModelType> Values(Func<ModelType, Expression> expression)
        {
            values = expression(Singleton<ModelType>.Default);
            return this;
        }

        /// <summary>
        /// Specifies expressions to filter query.
        /// </summary>
        /// <param name="expression">Lambda function returning expression.</param>
        public Update<ModelType> Where(Func<ModelType, Expression> expression)
        {
            where = expression(Singleton<ModelType>.Default);
            return this;
        }

        /// <summary>
        /// Specifies clauses to order by.
        /// </summary>
        /// <param name="order">Lambda function returning order clause.</param>
        public Update<ModelType> OrderBy(Func<ModelType, IList<Order>> order)
        {
            orderBy = order(Singleton<ModelType>.Default);
            return this;
        }
        public Update<ModelType> OrderBy(Func<ModelType, Order> order)
        {
            orderBy = new Order[] { order(Singleton<ModelType>.Default) };
            return this;
        }

        #region Limits and paging.
        /// <summary>
        /// Specifies number of top records to update.
        /// </summary>
        /// <param name="top">Number of rows to update.</param>
        public Update<ModelType> Top(int rowCount)
        {
            limit.Offset = 0;
            limit.RowCount = rowCount;
            return this;
        }

        /// <summary>
        /// Specifies limiting properties.
        /// </summary>
        /// <param name="offset">Offset to use.</param>
        /// <param name="rowCount">Number of rows to update.</param>
        public Update<ModelType> Limit(int offset, int rowCount)
        {
            limit.Offset = offset;
            limit.RowCount = rowCount;
            return this;
        }
        #endregion
    }
}
