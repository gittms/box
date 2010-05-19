using System;
using System.Collections.Generic;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents delete query.
    /// </summary>
    /// <typeparam name="ModelType">Type of querying model.</typeparam>
    public class Delete<ModelType> : Query<ModelType>
        where ModelType : class, IModel, new()
    {
        public Delete()
        {
            modelTable = Singleton<ModelType>.Default.IMapper().Table;
            type = QueryType.Delete;
        }

        /// <summary>
        /// Specifies expressions to filter query.
        /// </summary>
        /// <param name="expression">Lambda function returning expression.</param>
        public Delete<ModelType> Where(Func<ModelType, Expression> expression)
        {
            where = expression(Singleton<ModelType>.Default);
            return this;
        }
        /// <param name="expression">Expression to use.</param>
        public Delete<ModelType> Where(Expression expression)
        {
            where = expression;
            return this;
        }

        /// <summary>
        /// Specifies clauses to order by.
        /// </summary>
        /// <param name="order">Lambda function returning order clause.</param>
        public Delete<ModelType> OrderBy(Func<ModelType, IList<Order>> order)
        {
            orderBy = order(Singleton<ModelType>.Default);
            return this;
        }
        public Delete<ModelType> OrderBy(Func<ModelType, Order> order)
        {
            orderBy = new Order[] { order(Singleton<ModelType>.Default) };
            return this;
        }

        #region Limits and paging.
        /// <summary>
        /// Specifies number of top records to delete.
        /// </summary>
        /// <param name="top">Number of rows to delete.</param>
        public Delete<ModelType> Top(int rowCount)
        {
            limit.Offset = 0;
            limit.RowCount = rowCount;
            return this;
        }

        /// <summary>
        /// Specifies limiting properties.
        /// </summary>
        /// <param name="offset">Offset to use.</param>
        /// <param name="rowCount">Number of rows to delete.</param>
        public Delete<ModelType> Limit(int offset, int rowCount)
        {
            limit.Offset = offset;
            limit.RowCount = rowCount;
            return this;
        }
        #endregion
    }
}
