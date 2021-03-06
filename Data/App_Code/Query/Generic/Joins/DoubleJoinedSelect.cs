﻿using System;
using System.Collections.Generic;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents double joined select query.
    /// </summary>
    /// <typeparam name="ModelType">Type of querying model.</typeparam>
    public class DoubleJoinedSelect<ModelType, Join1ModelType, Join2ModelType> : Query<ModelType>
        where ModelType : class, IModel, new()
        where Join1ModelType : class, IModel, new()
        where Join2ModelType : class, IModel, new()
    {
        /// <summary>
        /// Specifies expression to get fields list.
        /// </summary>
        /// <param name="columns">Lambda function returning fields.</param>
        public DoubleJoinedSelect<ModelType, Join1ModelType, Join2ModelType> Fields(Func<ModelType, Join1ModelType, Join2ModelType, IList<Column>> columns)
        {
            fields = columns(Singleton<ModelType>.Default, Singleton<Join1ModelType>.Default, Singleton<Join2ModelType>.Default);
            return this;
        }
        public DoubleJoinedSelect<ModelType, Join1ModelType, Join2ModelType> Fields(Func<ModelType, Join1ModelType, Join2ModelType, Column> column)
        {
            fields = new Column[] { column(Singleton<ModelType>.Default, Singleton<Join1ModelType>.Default, Singleton<Join2ModelType>.Default) };
            return this;
        }

        /// <summary>
        /// Appends expressions to filter query.
        /// </summary>
        /// <param name="expression">Lambda function returning expression.</param>
        public DoubleJoinedSelect<ModelType, Join1ModelType, Join2ModelType> Where(Func<ModelType, Join1ModelType, Join2ModelType, Expression> expression)
        {
            where &= expression(Singleton<ModelType>.Default, Singleton<Join1ModelType>.Default, Singleton<Join2ModelType>.Default);
            return this;
        }
        /// <param name="expression">Expression to use.</param>
        public DoubleJoinedSelect<ModelType, Join1ModelType, Join2ModelType> Where(Expression expression)
        {
            where &= expression;
            return this;
        }
        /// <summary>
        /// Cleares query filter expression.
        /// </summary>
        public DoubleJoinedSelect<ModelType, Join1ModelType, Join2ModelType> ClearWhere()
        {
            where = null;
            return this;
        }

        /// <summary>
        /// Specifies clauses to order by.
        /// </summary>
        /// <param name="order">Lambda function returning order clause.</param>
        public DoubleJoinedSelect<ModelType, Join1ModelType, Join2ModelType> OrderBy(Func<ModelType, Join1ModelType, Join2ModelType, IList<Order>> order)
        {
            orderBy = order(Singleton<ModelType>.Default, Singleton<Join1ModelType>.Default, Singleton<Join2ModelType>.Default);
            return this;
        }
        public DoubleJoinedSelect<ModelType, Join1ModelType, Join2ModelType> OrderBy(Func<ModelType, Join1ModelType, Join2ModelType, Order> order)
        {
            orderBy = new Order[] { order(Singleton<ModelType>.Default, Singleton<Join1ModelType>.Default, Singleton<Join2ModelType>.Default) };
            return this;
        }

        /// <summary>
        /// Specifies columns to group by.
        /// </summary>
        /// <param name="group">Lambda function returning group by.</param>
        public DoubleJoinedSelect<ModelType, Join1ModelType, Join2ModelType> GroupBy(Func<ModelType, Join1ModelType, Join2ModelType, IList<Column>> group)
        {
            groupBy = group(Singleton<ModelType>.Default, Singleton<Join1ModelType>.Default, Singleton<Join2ModelType>.Default);
            return this;
        }
        public DoubleJoinedSelect<ModelType, Join1ModelType, Join2ModelType> GroupBy(Func<ModelType, Join1ModelType, Join2ModelType, Column> group)
        {
            groupBy = new Column[] { group(Singleton<ModelType>.Default, Singleton<Join1ModelType>.Default, Singleton<Join2ModelType>.Default) };
            return this;
        }

        /// <summary>
        /// Specifies is it required to select distinct rows.
        /// </summary>
        public DoubleJoinedSelect<ModelType, Join1ModelType, Join2ModelType> Distinct(bool distinct = true)
        {
            this.distinct = distinct;
            return this;
        }

        #region Limits and paging.
        /// <summary>
        /// Specifies number of top records to select.
        /// </summary>
        /// <param name="top">Number of rows to select.</param>
        public DoubleJoinedSelect<ModelType, Join1ModelType, Join2ModelType> Top(int rowCount)
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
        public DoubleJoinedSelect<ModelType, Join1ModelType, Join2ModelType> Limit(int offset, int rowCount)
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
        public DoubleJoinedSelect<ModelType, Join1ModelType, Join2ModelType> Page(int rowsPerPage, int page)
        {
            limit.Offset = rowsPerPage * page;
            limit.RowCount = rowsPerPage;
            return this;
        } 
        #endregion

        #region Joins.
        private TripleJoinedSelect<ModelType, Join1ModelType, Join2ModelType, Join3ModelType> Join<Join3ModelType>(Func<ModelType, Join1ModelType, Join2ModelType, Join3ModelType, Expression> expression, JoinType type)
            where Join3ModelType : class, IModel, new()
        {
            joins.Add(new Join()
            {
                Clause = expression(Singleton<ModelType>.Default, Singleton<Join1ModelType>.Default, Singleton<Join2ModelType>.Default, Singleton<Join3ModelType>.Default),
                Table = Singleton<Join3ModelType>.Default.IMapper().Table,
                Type = type,
            });
            return this.Clone<TripleJoinedSelect<ModelType, Join1ModelType, Join2ModelType, Join3ModelType>>();
        }

        /// <summary>
        /// Performs inner join on given model type.
        /// </summary>
        /// <typeparam name="JoinModelType">Model type to join.</typeparam>
        /// <param name="expression">Lambda function returning clause.</param>
        public TripleJoinedSelect<ModelType, Join1ModelType, Join2ModelType, Join3ModelType> InnerJoin<Join3ModelType>(Func<ModelType, Join1ModelType, Join2ModelType, Join3ModelType, Expression> expression)
            where Join3ModelType : class, IModel, new()
        {
            return this.Join<Join3ModelType>(expression, JoinType.Inner);
        }

        /// <summary>
        /// Performs right join on given model type.
        /// </summary>
        /// <typeparam name="JoinModelType">Model type to join.</typeparam>
        /// <param name="expression">Lambda function returning clause.</param>
        public TripleJoinedSelect<ModelType, Join1ModelType, Join2ModelType, Join3ModelType> RightJoin<Join3ModelType>(Func<ModelType, Join1ModelType, Join2ModelType, Join3ModelType, Expression> expression)
            where Join3ModelType : class, IModel, new()
        {
            return this.Join<Join3ModelType>(expression, JoinType.Right);
        }

        /// <summary>
        /// Performs left join on given model type.
        /// </summary>
        /// <typeparam name="JoinModelType">Model type to join.</typeparam>
        /// <param name="expression">Lambda function returning clause.</param>
        public TripleJoinedSelect<ModelType, Join1ModelType, Join2ModelType, Join3ModelType> LeftJoin<Join3ModelType>(Func<ModelType, Join1ModelType, Join2ModelType, Join3ModelType, Expression> expression)
            where Join3ModelType : class, IModel, new()
        {
            return this.Join<Join3ModelType>(expression, JoinType.Left);
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
