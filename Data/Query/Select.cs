using System;

namespace Definitif.Data.Query
{
    /// <summary>
    /// Represents select query.
    /// </summary>
    /// <typeparam name="ModelType">Type of querying model.</typeparam>
    public class Select<ModelType> : Query<ModelType>
        where ModelType : class, IModel, new()
    {
        protected Expression where = null;

        /// <summary>
        /// Specifies expressions to filter query.
        /// </summary>
        /// <param name="expression">Lambda funtion returning expression.</param>
        public Select<ModelType> Where(Func<ModelType, Expression> expression)
        {
            where = expression(default(ModelType));
            return this;
        }

        /// <summary>
        /// Specifies columns to order by.
        /// </summary>
        /// <param name="order">Lambda function returning order clause.</param>
        public Select<ModelType> OrderBy(Func<ModelType, Order> order)
        {
            orderBy = order(default(ModelType));
            return this;
        }

        /// <summary>
        /// Specifies columns to group by.
        /// </summary>
        /// <param name="group">Lambda function returning group by.</param>
        public Select<ModelType> GroupBy(Func<ModelType, Group> group)
        {
            groupBy = group(default(ModelType));
            return this;
        }

        #region Limits and paging.
        /// <summary>
        /// Specifies number of top records to select.
        /// </summary>
        /// <param name="top">Number of rows to select.</param>
        public Select<ModelType> Top(int rowCount)
        {
            limit.Offset = 0;
            limit.RowCount = rowCount;
            return this;
        }

        /// <summary>
        /// Specifies limiting properties.
        /// </summary>
        /// <param name="offset">Offset to use.</param>
        /// <param name="rowCount">Number of rows to select.</param>
        public Select<ModelType> Limit(int offset, int rowCount)
        {
            limit.Offset = offset;
            limit.RowCount = rowCount;
            return this;
        }

        /// <summary>
        /// Specifies limiting properties as paging.
        /// </summary>
        /// <param name="rowsPerPage">Number of rows per page.</param>
        /// <param name="page">Page number to select.</param>
        public Select<ModelType> Page(int rowsPerPage, int page)
        {
            limit.Offset = rowsPerPage * page;
            limit.RowCount = rowsPerPage;
            return this;
        } 
        #endregion
    }
}
