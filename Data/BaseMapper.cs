using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Definitif.Data
{
    /// <summary>
    /// Base object-relational mapper class.
    /// </summary>
    /// <typeparam name="ModelType">Specific IModel type to be associated with this mapper.</typeparam>
    public abstract class BaseMapper<ModelType> : IMapper
        where ModelType : class, IModel, new()
    {
        /// <summary>
        /// Reads an object with specified Id from database.
        /// </summary>
        /// <param name="id">Model Id.</param>
        /// <returns>Model instance.</returns>
        public ModelType Read(Id id)
        {
            return this.Read(null, id);
        }

        /// <summary>
        /// Reads an object with specified Id from database.
        /// </summary>
        /// <param name="connection">DbConnection to use. If null, new connection will be created.</param>
        /// <param name="id">Model Id.</param>
        /// <returns>Model instance.</returns>
        public virtual ModelType Read(DbConnection connection, Id id)
        {
            if (id.Equals(Id.Empty)) throw new ArgumentException(String.Format("Cannot read model by empty Id ({0}).", typeof(ModelType).ToString()));
            
            bool policy = this.InitConnection(ref connection);
            DbCommand command = this.ReadCommand(id);
            command.Connection = connection;

            ModelType result = this.Read(command);
            if (!policy) connection.Close();
            return result;
        }

        /// <summary>
        /// Executes an DbCommand and reads a single object.
        /// </summary>
        /// <param name="command">DbCommand to execute.</param>
        /// <returns>Filled Model instance.</returns>
        protected ModelType Read(DbCommand command)
        {
            ModelType result = default(ModelType);

            IDataReader reader = this.ExecuteReader(command);
            if (reader.Read()) result = this.ReadObject(reader);
            reader.Close();

            return result;
        }

        /// <summary>
        /// Reads all objects of current type from database.
        /// </summary>
        /// <returns>List of Model objects.</returns>
        public List<ModelType> ReadAll()
        {
            return this.ReadAll(null);
        }

        /// <summary>
        /// Reads all objects of current type from database.
        /// </summary>
        /// <param name="connection">DbConnection to use. If null, new connection will be created.</param>
        /// <returns>List of Model objects.</returns>
        public virtual List<ModelType> ReadAll(DbConnection connection)
        {
            bool policy = this.InitConnection(ref connection);
            DbCommand command = this.ReadAllCommand();
            command.Connection = connection;

            List<ModelType> result = this.ReadMultiple(command);
            if (!policy) connection.Close();
            return result;
        }

        /// <summary>
        /// Writes an object to database.
        /// </summary>
        /// <param name="obj">Object to write.</param>
        public void Write(IModel obj)
        {
            this.Write(obj as ModelType);
        }

        /// <summary>
        /// Writes an object to database.
        /// </summary>
        /// <param name="obj">Object to write.</param>
        public virtual void Write(ModelType obj)
        {
            this.Write(null, obj);
        }

        /// <summary>
        /// Writes an object to database.
        /// </summary>
        /// <param name="connection">DbConnection to use. If null, new connection will be created.</param>
        /// <param name="obj">Object to write.</param>
        public void Write(DbConnection connection, IModel obj)
        {
            this.Write(connection, obj as ModelType);
        }

        /// <summary>
        /// Writes an object to database.
        /// </summary>
        /// <param name="connection">DbConnection to use. If null, new connection will be created.</param>
        /// <param name="obj">Object to write.</param>
        public virtual void Write(DbConnection connection, ModelType obj)
        {
            bool policy = this.InitConnection(ref connection);
            if (obj.Id.Equals(Id.Empty)) this.Insert(connection, obj);
            else this.Update(connection, obj);

            if (!policy) connection.Close();
        }

        /// <summary>
        /// Inserts an object to database, then updates its Id.
        /// </summary>
        /// <param name="connection">DbConnection to use.</param>
        /// <param name="obj">Object to insert.</param>
        protected virtual void Insert(DbConnection connection, ModelType obj)
        {
            bool policy = this.InitConnection(ref connection);

            List<DbCommand> commands = this.InsertCommands(obj);

            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Connection = connection;

                this.ExecuteNonQuery(commands[i]);

                // If this is the first command, executing it
                // and getting last ID.
                // REFACTORING: Possibly make every command read last ID until it is available.
                if (i == 0)
                {
                    obj.Id = this.ReadLastId(connection);
                }
            }

            if (!policy) connection.Close();
        }

        /// <summary>
        /// Updates an object in database, then updates its version.
        /// </summary>
        /// <param name="connection">DbConnection to use.</param>
        /// <param name="obj">Object to update.</param>
        protected virtual void Update(DbConnection connection, ModelType obj)
        {
            bool policy = this.InitConnection(ref connection);
            List<DbCommand> commands = this.UpdateCommands(obj);

            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Connection = connection;

                int result = this.ExecuteNonQuery(commands[i]);

                // If this is the first command, verifying for
                // number of rows affected.
                // REFACTORING: Possibly make every command verify number of rows.
                if (i == 0 && result == 0)
                {
                    if (!policy) connection.Close();
                    throw new DBConcurrencyException();
                }
            }
            obj.Version++;

            if (!policy) connection.Close();
        }

        /// <summary>
        /// Deletes an object from database.
        /// </summary>
        /// <param name="obj">Object to delete.</param>
        public void Delete(IModel obj)
        {
            this.Delete(obj as ModelType);
        }

        /// <summary>
        /// Deletes an object from database.
        /// </summary>
        /// <param name="obj">Object to delete.</param>
        public void Delete(ModelType obj)
        {
            this.Delete(null, obj);
        }

        /// <summary>
        /// Deletes an object from database.
        /// </summary>
        /// <param name="connection">DbConnection to use. If null, new connection will be created.</param>
        /// <param name="obj">Object to delete.</param>
        public void Delete(DbConnection connection, IModel obj)
        {
            this.Delete(connection, obj as ModelType);
        }

        /// <summary>
        /// Deletes an object from database.
        /// </summary>
        /// <param name="connection">DbConnection to use.</param>
        /// <param name="obj">Object to delete.</param>
        public virtual void Delete(DbConnection connection, ModelType obj)
        {
            if (obj.Id == Id.Empty)
            {
                throw new ArgumentException(String.Format("Cannot delete model ({0}) with empty Id.", typeof(ModelType).ToString()));
            }

            bool policy = this.InitConnection(ref connection);

            List<DbCommand> commands = this.DeleteCommands(obj);

            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Connection = connection;

                int result = this.ExecuteNonQuery(commands[i]);

                // If this is the first command, verifying for
                // number of rows affected.
                // REFACTORING: Possibly make every command verify number of rows.
                if (i == 0 && result == 0)
                {
                    if (!policy) connection.Close();
                    throw new DBConcurrencyException();
                }
            }
            obj.Id = Id.Empty;
            obj.Version = 0;

            if (!policy) connection.Close();
        }

        /// <summary>
        /// When overridden in derived class, returns a Model filled from executed IDataReader.
        /// </summary>
        /// <param name="reader">Executed IDataReader.</param>
        /// <returns>Filled Model instance.</returns>
        public abstract ModelType ReadObject(IDataReader reader);

        /// <summary>
        /// When overridden in derived class, returns a Model filled from executed IDataReader.
        /// </summary>
        /// <param name="reader">Executed IDataReader.</param>
        /// <returns>Filled Model instance.</returns>
        /// <param name="fieldPrefix">Prefix that should be added to database field names.</param>
        public virtual ModelType ReadObject(IDataReader reader, string fieldPrefix)
        {
            return ReadObject(reader);
        }

        /// <summary>
        /// Reads Id and Version into model from executed IDataReader.
        /// </summary>
        /// <param name="model">Model to read into.</param>
        /// <param name="reader">Executed IDataReader instance.</param>
        protected void FillBase(IModel model, IDataReader reader)
        {
            FillBase(model, reader, "");
        }

        /// <summary>
        /// Reads Id and Version into model from executed IDataReader.
        /// </summary>
        /// <param name="model">Model to read into.</param>
        /// <param name="reader">Executed IDataReader instance.</param>
        /// <param name="fieldPrefix">Prefix that should be added to database field names.</param>
        protected virtual void FillBase(IModel model, IDataReader reader, string fieldPrefix)
        {
            model.Id = new Id(reader[fieldPrefix + "Id"]);
            model.Version = (int)reader[fieldPrefix + "Version"];
        }

        /// <summary>
        /// Reads last inserted Id from database.
        /// </summary>
        /// <param name="connection">DbConnection used for inserting the Id. Can not be null and should be opened.</param>
        /// <returns>Id instance.</returns>
        protected virtual Id ReadLastId(DbConnection connection)
        {
            DbCommand command = this.LastIdCommand();
            command.Connection = connection;

            return new Id(command.ExecuteScalar());
        }

        /// <summary>
        /// Executes an DbCommand and returns the number of rows affected.
        /// </summary>
        /// <param name="command">DbCommand to execute.</param>
        /// <returns>Number of rows affected.</returns>
        protected int ExecuteNonQuery(DbCommand command)
        {
            DbConnection connection = command.Connection;
            bool policy = this.InitConnection(ref connection);
            command.Connection = connection;

            int result = command.ExecuteNonQuery();

            if (!policy) connection.Close();
            return result;
        }

        /// <summary>
        /// Executes and DbCommand and returns executed reader.
        /// </summary>
        /// <param name="command">DbCommand to execute.</param>
        /// <returns>Number of rows affected.</returns>
        protected IDataReader ExecuteReader(DbCommand command)
        {
            DbConnection connection = command.Connection;
            bool policy = this.InitConnection(ref connection);
            command.Connection = connection;

            IDataReader result = command.ExecuteReader(policy ? 
                CommandBehavior.Default : 
                CommandBehavior.CloseConnection);
            return result;
        }

        /// <summary>
        /// Executes an DbCommand and reads all objects from the result.
        /// </summary>
        /// <param name="command">DbCommand to execute.</param>
        /// <returns>List of Model instances.</returns>
        protected List<ModelType> ReadMultiple(DbCommand command)
        {
            List<ModelType> result = new List<ModelType>();

            IDataReader reader = this.ExecuteReader(command);
            while (reader.Read()) result.Add(this.ReadObject(reader));
            reader.Close();

            return result;
        }

        /// <summary>
        /// Gets an DbConnection.
        /// </summary>
        /// <returns>DbConnection.</returns>
        public abstract DbConnection GetConnection();

        /// <summary>
        /// Initializes DbConnection and returns behaviour
        /// policy.
        /// </summary>
        /// <param name="connection">Connection to initialize.</param>
        /// <returns>True if connection should stay open, overwise false.</returns>
        protected bool InitConnection(ref DbConnection connection)
        {
            // Initializing connection.
            if (connection == null)
            {
                connection = this.GetConnection();

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                // If connection has been created using
                // active transaction - it's currently
                // open, but needs to be closed too.
                return false;
            }

            // Opening connection if needed.
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                return false;
            }

            return true;
        }

        /// <summary>
        /// When overridden in derived class, returns the query for reading a single object from database with specified Id.
        /// </summary>
        /// <param name="id">Object Id to read.</param>
        /// <returns>Initialized DbCommand instance.</returns>
        protected abstract DbCommand ReadCommand(Id id);

        /// <summary>
        /// When overridden in derived class, returns the query for reading all objects of current type from database.
        /// </summary>
        /// <returns>Initialized DbCommand instance.</returns>
        protected abstract DbCommand ReadAllCommand();

        /// <summary>
        /// When overridden in derived class, returns queries for writing new object to database.
        /// </summary>
        /// <param name="obj">Object to be inserted.</param>
        /// <returns>List of DbCommand instances.</returns>
        protected abstract List<DbCommand> InsertCommands(ModelType obj);

        /// <summary>
        /// When overridden in derived class, returns queries for updating an object in database.
        /// </summary>
        /// <param name="obj">Object to be updated.</param>
        /// <returns>List of DbCommand instances.</returns>
        protected abstract List<DbCommand> UpdateCommands(ModelType obj);

        /// <summary>
        /// When overridden in derived class, returns queries for deleting an object from database.
        /// </summary>
        /// <param name="obj">Object to be deleted.</param>
        /// <returns>List of DbCommand instances.</returns>
        protected abstract List<DbCommand> DeleteCommands(ModelType obj);

        /// <summary>
        /// Returns a query for reading the last inserted Id from database.
        /// </summary>
        /// <returns>Initialized DbCommand instance.</returns>
        protected abstract DbCommand LastIdCommand();
    }
}
