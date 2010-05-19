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
        public Insert()
        {
            modelTable = Singleton<ModelType>.Default.IMapper().Table;
            type = QueryType.Insert;
        }

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
