using System;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents insert query.
    /// </summary>
    /// <typeparam name="ModelType">Type of querying model.</typeparam>
    public class Insert<ModelType> : Query<ModelType>
        where ModelType : class, IModel, new()
    {
        protected Expression values = null;

        /// <summary>
        /// Specifies values to insert.
        /// </summary>
        /// <param name="expression">Lambda function returning expression.</param>
        public Insert<ModelType> Values(Func<ModelType, Expression> expression)
        {
            values = expression(Singleton<ModelType>.Default);
            return this;
        }
    }
}
