using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Definitif.Data.Query;

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
            command.CommandText = this.database.Drawer.Identity;
            return command;
        }

        protected override DbCommand ReadCommand(Id id)
        {
            return this.database.GetCommand(new Select<ModelType>().Where(m => m.C.Id == id.Value));
        }

        protected override DbCommand ReadAllCommand()
        {
            return this.database.GetCommand(new Select<ModelType>());
        }
    }
}
