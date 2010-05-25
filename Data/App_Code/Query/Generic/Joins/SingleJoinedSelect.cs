using System;
using System.Collections.Generic;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents single joined select query.
    /// </summary>
    /// <typeparam name="ModelType">Type of querying model.</typeparam>
    public class SingleJoinedSelect<ModelType, JoinModelType> : Query<ModelType>
        where ModelType : class, IModel, new()
        where JoinModelType : class, IModel, new()
    {
        /// <summary>
        /// Specifies expression to get fields list.
        /// </summary>
        /// <param name="columns">Lambda function returning fields.</param>
        public SingleJoinedSelect<ModelType, JoinModelType> Fields(Func<ModelType, JoinModelType, IList<Column>> columns)
        {
            fields = columns(Singleton<ModelType>.Default, Singleton<JoinModelType>.Default);
            return this;
        }
        public SingleJoinedSelect<ModelType, JoinModelType> Fields(Func<ModelType, JoinModelType, Column> column)
        {
            fields = new Column[] { column(Singleton<ModelType>.Default, Singleton<JoinModelType>.Default) };
            return this;
        }

        /// <summary>
        /// Specifies expressions to filter query.
        /// </summary>
        /// <param name="expression">Lambda function returning expression.</param>
        public SingleJoinedSelect<ModelType, JoinModelType> Where(Func<ModelType, JoinModelType, Expression> expression)
        {
            where = expression(Singleton<ModelType>.Default, Singleton<JoinModelType>.Default);
            return this;
        }
        /// <param name="expression">Expression to use.</param>
        public SingleJoinedSelect<ModelType, JoinModelType> Where(Expression expression)
        {
            where = expression;
            return this;
        }

        /// <summary>
        /// Specifies clauses to order by.
        /// </summary>
        /// <param name="order">Lambda function returning order clause.</param>
        public SingleJoinedSelect<ModelType, JoinModelType> OrderBy(Func<ModelType, JoinModelType, IList<Order>> order)
        {
            orderBy = order(Singleton<ModelType>.Default, Singleton<JoinModelType>.Default);
            return this;
        }
        public SingleJoinedSelect<ModelType, JoinModelType> OrderBy(Func<ModelType, JoinModelType, Order> order)
        {
            orderBy = new Order[] { order(Singleton<ModelType>.Default, Singleton<JoinModelType>.Default) };
            return this;
        }

        /// <summary>
        /// Specifies columns to group by.
        /// </summary>
        /// <param name="group">Lambda function returning group by.</param>
        public SingleJoinedSelect<ModelType, JoinModelType> GroupBy(Func<ModelType, JoinModelType, IList<Column>> group)
        {
            groupBy = group(Singleton<ModelType>.Default, Singleton<JoinModelType>.Default);
            return this;
        }
        public SingleJoinedSelect<ModelType, JoinModelType> GroupBy(Func<ModelType, JoinModelType, Column> group)
        {
            groupBy = new Column[] { group(Singleton<ModelType>.Default, Singleton<JoinModelType>.Default) };
            return this;
        }

        #region Limits and paging.
        /// <summary>
        /// Specifies number of top records to select.
        /// </summary>
        /// <param name="top">Number of rows to select.</param>
        public SingleJoinedSelect<ModelType, JoinModelType> Top(int rowCount)
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
        public SingleJoinedSelect<ModelType, JoinModelType> Limit(int offset, int rowCount)
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
        public SingleJoinedSelect<ModelType, JoinModelType> Page(int rowsPerPage, int page)
        {
            limit.Offset = rowsPerPage * page;
            limit.RowCount = rowsPerPage;
            return this;
        } 
        #endregion

        #region Joins.
        private DoubleJoinedSelect<ModelType, JoinModelType, Join2ModelType> Join<Join2ModelType>(Func<ModelType, JoinModelType, Join2ModelType, Expression> expression, JoinType type)
            where Join2ModelType : class, IModel, new()
        {
            joins.Add(new Join()
            {
                Clause = expression(Singleton<ModelType>.Default, Singleton<JoinModelType>.Default, Singleton<Join2ModelType>.Default),
                Table = Singleton<Join2ModelType>.Default.IMapper().Table,
                Type = type,
            });
            return this.Clone<DoubleJoinedSelect<ModelType, JoinModelType, Join2ModelType>>();
        }

        /// <summary>
        /// Performs inner join on given model type.
        /// </summary>
        /// <typeparam name="JoinModelType">Model type to join.</typeparam>
        /// <param name="expression">Lambda function returning clause.</param>
        public DoubleJoinedSelect<ModelType, JoinModelType, Join2ModelType> InnerJoin<Join2ModelType>(Func<ModelType, JoinModelType, Join2ModelType, Expression> expression)
            where Join2ModelType : class, IModel, new()
        {
            return this.Join<Join2ModelType>(expression, JoinType.Inner);
        }

        /// <summary>
        /// Performs right join on given model type.
        /// </summary>
        /// <typeparam name="JoinModelType">Model type to join.</typeparam>
        /// <param name="expression">Lambda function returning clause.</param>
        public DoubleJoinedSelect<ModelType, JoinModelType, Join2ModelType> RightJoin<Join2ModelType>(Func<ModelType, JoinModelType, Join2ModelType, Expression> expression)
            where Join2ModelType : class, IModel, new()
        {
            return this.Join<Join2ModelType>(expression, JoinType.Right);
        }

        /// <summary>
        /// Performs left join on given model type.
        /// </summary>
        /// <typeparam name="JoinModelType">Model type to join.</typeparam>
        /// <param name="expression">Lambda function returning clause.</param>
        public DoubleJoinedSelect<ModelType, JoinModelType, Join2ModelType> LeftJoin<Join2ModelType>(Func<ModelType, JoinModelType, Join2ModelType, Expression> expression)
            where Join2ModelType : class, IModel, new()
        {
            return this.Join<Join2ModelType>(expression, JoinType.Left);
        }
        #endregion

        #region Queries.
        /// <summary>
        /// Reads query result into models array.
        /// </summary>
        /// <returns>Array of models.</returns>
        public ModelType[] Read()
        {
            return base.ReadModels();
        }
        #endregion
    }
}
