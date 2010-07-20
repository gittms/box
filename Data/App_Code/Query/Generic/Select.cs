using System;
using System.Collections.Generic;
using System.Data;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents select query.
    /// </summary>
    /// <typeparam name="ModelType">Type of querying model.</typeparam>
    public class Select<ModelType> : Query<ModelType>
        where ModelType : class, IModel, new()
    {
        public Select()
        {
            modelTable = Singleton<ModelType>.Default.IMapper().Table;
        }

        /// <summary>
        /// Specifies expression to get fields list.
        /// </summary>
        /// <param name="columns">Lambda function returning fields.</param>
        public Select<ModelType> Fields(Func<ModelType, IList<Column>> columns)
        {
            fields = columns(Singleton<ModelType>.Default);
            return this;
        }
        public Select<ModelType> Fields(Func<ModelType, Column> column)
        {
            fields = new Column[] { column(Singleton<ModelType>.Default) };
            return this;
        }

        /// <summary>
        /// Appends expressions to filter query.
        /// </summary>
        /// <param name="expression">Lambda function returning expression.</param>
        public Select<ModelType> Where(Func<ModelType, Expression> expression)
        {
            where &= expression(Singleton<ModelType>.Default);
            return this;
        }
        /// <param name="expression">Expression to use.</param>
        public Select<ModelType> Where(Expression expression)
        {
            where &= expression;
            return this;
        }
        /// <summary>
        /// Cleares query filter expression.
        /// </summary>
        public Select<ModelType> ClearWhere()
        {
            where = null;
            return this;
        }

        /// <summary>
        /// Specifies clauses to order by.
        /// </summary>
        /// <param name="order">Lambda function returning order clause.</param>
        public Select<ModelType> OrderBy(Func<ModelType, IList<Order>> order)
        {
            orderBy = order(Singleton<ModelType>.Default);
            return this;
        }
        public Select<ModelType> OrderBy(Func<ModelType, Order> order)
        {
            orderBy = new Order[] { order(Singleton<ModelType>.Default) };
            return this;
        }

        /// <summary>
        /// Specifies columns to group by.
        /// </summary>
        /// <param name="group">Lambda function returning group by.</param>
        public Select<ModelType> GroupBy(Func<ModelType, IList<Column>> group)
        {
            groupBy = group(Singleton<ModelType>.Default);
            return this;
        }
        public Select<ModelType> GroupBy(Func<ModelType, Column> group)
        {
            groupBy = new Column[] { group(Singleton<ModelType>.Default) };
            return this;
        }

        /// <summary>
        /// Specifies is it required to select distinct rows.
        /// </summary>
        public Select<ModelType> Distinct(bool distinct = true)
        {
            this.distinct = distinct;
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

        #region Joins.
        private SingleJoinedSelect<ModelType, JoinModelType> Join<JoinModelType>(Func<ModelType, JoinModelType, Expression> expression, JoinType type)
            where JoinModelType : class, IModel, new()
        {
            joins = new List<Join>() { new Join()
            {
                Clause = expression(Singleton<ModelType>.Default, Singleton<JoinModelType>.Default),
                Table = Singleton<JoinModelType>.Default.IMapper().Table,
                Type = type,
            } };
            return this.Clone<SingleJoinedSelect<ModelType, JoinModelType>>();
        }

        /// <summary>
        /// Performs inner join on given model type.
        /// </summary>
        /// <typeparam name="JoinModelType">Model type to join.</typeparam>
        /// <param name="expression">Lambda function returning clause.</param>
        public SingleJoinedSelect<ModelType, JoinModelType> InnerJoin<JoinModelType>(Func<ModelType, JoinModelType, Expression> expression)
            where JoinModelType : class, IModel, new()
        {
            return this.Join<JoinModelType>(expression, JoinType.Inner);
        }

        /// <summary>
        /// Performs right join on given model type.
        /// </summary>
        /// <typeparam name="JoinModelType">Model type to join.</typeparam>
        /// <param name="expression">Lambda function returning clause.</param>
        public SingleJoinedSelect<ModelType, JoinModelType> RightJoin<JoinModelType>(Func<ModelType, JoinModelType, Expression> expression)
            where JoinModelType : class, IModel, new()
        {
            return this.Join<JoinModelType>(expression, JoinType.Right);
        }

        /// <summary>
        /// Performs left join on given model type.
        /// </summary>
        /// <typeparam name="JoinModelType">Model type to join.</typeparam>
        /// <param name="expression">Lambda function returning clause.</param>
        public SingleJoinedSelect<ModelType, JoinModelType> LeftJoin<JoinModelType>(Func<ModelType, JoinModelType, Expression> expression)
            where JoinModelType : class, IModel, new()
        {
            return this.Join<JoinModelType>(expression, JoinType.Left);
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
        /// <summary>
        /// Reads first query result into model.
        /// </summary>
        /// <returns>First model of result.</returns>
        public ModelType ReadFirst()
        {
            return base.ReadFirstModel();
        }
        #endregion
    }
}
