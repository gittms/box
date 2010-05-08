using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Definitif.Data.ObjectSql;
using Definitif.Data.ObjectSql.Query;

namespace Definitif.Data
{
    /// <summary>
    /// Represents common mapper for models.
    /// </summary>
    public abstract class Mapper<ModelType> : BaseMapper<ModelType>
        where ModelType : class, IModel, new()
    {
        protected Database database;
        protected Table table;

        /// <summary>
        /// Gets common mapping table.
        /// </summary>
        public Table Table
        {
            get { return this.table; }
        }

        public override DbConnection GetConnection()
        {
            return this.database.GetConnection();
        }

        protected override DbCommand LastIdCommand()
        {
            DbCommand command = this.database.GetCommand();
            command.CommandText = this.database.Drawer.IDENTITY;
            return command;
        }

        protected override DbCommand ReadCommand(Id id)
        {
            return this.database.GetCommand(
                new Select(table["*"]).Where(table["Id"] == id.Value));
        }

        protected override DbCommand ReadAllCommand()
        {
            return this.database.GetCommand(
                new Select(table["*"]));
        }

        /// <summary>
        /// Filters models using given expressions.
        /// </summary>
        /// <param name="parameters">Expressions to filter.</param>
        public List<ModelType> Filter(params IExpression[] parameters)
        {
            Select query = new Select(table["*"]);
            foreach (IExpression expression in parameters)
            {
                query.WHERE.Add(expression);
            }

            return this.ReadMultiple(this.database.GetCommand(query));
        }

        /// <summary>
        /// Filters models by given field.
        /// </summary>
        /// <param name="field">Field to filter by.</param>
        /// <param name="id">Id value to filter.</param>
        public List<ModelType> Filter(string field, Id id)
        {
            if (id.Equals(Id.Empty)) return new List<ModelType>();
            else return this.Filter(table[field] == id.Value);
        }
        /// <summary>
        /// Filters models by given field.
        /// </summary>
        /// <param name="field">Field to filter by.</param>
        /// <param name="value">Value to filter.</param>
        public List<ModelType> Filter(string field, object value)
        {
            return this.Filter(table[field] == value);
        }
    }
}
