using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents abstract generic query.
    /// </summary>
    /// <typeparam name="ModelType">Type of querying model.</typeparam>
    public abstract class Query<ModelType> : Query
        where ModelType : class, IModel, new()
    {
        /// <summary>
        /// Draws query to string.
        /// </summary>
        /// <returns>String representation of query.</returns>
        public override string ToString()
        {
            // Getting drawer model was initialized with.
            Drawer drawer = modelTable.Database.Drawer;

            return this.Draw(drawer);
        }

        /// <summary>
        /// Executes reader for current query.
        /// </summary>
        /// <returns>Executed data reader.</returns>
        public IDataReader ExecuteReader()
        {
            return modelTable.Database.GetCommand(this, true)
                .ExecuteReader(CommandBehavior.CloseConnection);
        }
        /// <summary>
        /// Executes query and return number of rows affected.
        /// </summary>
        /// <returns>Number of rows affected.</returns>
        public int Execute()
        {
            DbCommand command = modelTable.Database.GetCommand(this, true);
            int result = command.ExecuteNonQuery();
            command.Connection.Close();
            return result;
        }
        /// <summary>
        /// Reads query result into array of models.
        /// </summary>
        /// <returns>Array of models.</returns>
        protected ModelType[] ReadModels()
        {
            // Getting mapper for model.
            BaseMapper<ModelType> mapper = Singleton<ModelType>.Default.IMapper() as BaseMapper<ModelType>;

            List<ModelType> result = new List<ModelType>();
            IDataReader reader = this.ExecuteReader();
            while (reader.Read())
            {
                result.Add(mapper.ReadObject(reader));
            }
            reader.Close();
            return result.ToArray();
        }
    }
}
