using System;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents delete query.
    /// </summary>
    /// <typeparam name="ModelType">Type of querying model.</typeparam>
    public class Delete<ModelType> : Query<ModelType>
        where ModelType : class, IModel, new()
    {
        protected Expression where = null;

        /// <summary>
        /// Specifies expressions to filter query.
        /// </summary>
        /// <param name="expression">Lambda function returning expression.</param>
        public Delete<ModelType> Where(Func<ModelType, Expression> expression)
        {
            where = expression(default(ModelType));
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

        protected override string Draw(Drawer drawer)
        {
            throw new NotImplementedException();
        }
    }
}
