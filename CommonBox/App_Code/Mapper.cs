using System;
using System.Collections.Generic;
using System.Data;

namespace Definitif.Data.CommonBox
{
    /// <summary>
    /// Base object-relational mapper class.
    /// </summary>
    /// <typeparam name="ModelType">Specific IModel type to be associated with this mapper.</typeparam>
    public abstract class Mapper<ModelType> : IMapper
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
        /// <param name="connection">IDbConnection to use. If null, new connection will be created.</param>
        /// <param name="id">Model Id.</param>
        /// <returns>Model instance.</returns>
        public virtual ModelType Read(IDbConnection connection, Id id)
        {
            bool policy = this.InitConnection(ref connection);
            IDbCommand command = this.ReadCommand(id);
            command.Connection = connection;

            ModelType result = this.Read(command);
            if (!policy) connection.Close();
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
        /// <param name="connection">IDbConnection to use. If null, new connection will be created.</param>
        /// <returns>List of Model objects.</returns>
        public virtual List<ModelType> ReadAll(IDbConnection connection)
        {
            bool policy = this.InitConnection(ref connection);
            IDbCommand command = this.ReadAllCommand();
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
            this.Write(null, null, obj);
        }

        /// <summary>
        /// Writes an object to database.
        /// </summary>
        /// <param name="connection">IDbConnection to use. If null, new connection will be created.</param>
        /// <param name="transaction">IDbTransaction to use. If null, no transaction will ne used.</param>
        /// <param name="obj">Object to write.</param>
        public void Write(IDbConnection connection, IDbTransaction transaction, IModel obj)
        {
            this.Write(connection, transaction, obj as ModelType);
        }

        /// <summary>
        /// Writes an object to database.
        /// </summary>
        /// <param name="connection">IDbConnection to use. If null, new connection will be created.</param>
        /// <param name="transaction">IDbTransaction to use. If null, no transaction will ne used.</param>
        /// <param name="obj">Object to write.</param>
        public virtual void Write(IDbConnection connection, IDbTransaction transaction, ModelType obj)
        {
            bool policy = this.InitConnection(ref connection);
            if (obj.Id.Equals(Id.Empty)) this.Insert(connection, transaction, obj);
            else this.Update(connection, transaction, obj);

            if (!policy) connection.Close();
        }

        /// <summary>
        /// Inserts an object to database, then updates its Id.
        /// </summary>
        /// <param name="connection">IDbConnection to use.</param>
        /// <param name="transaction">IDbTransaction to use. If null, no transaction will ne used.</param>
        /// <param name="obj">Object to insert.</param>
        protected virtual void Insert(IDbConnection connection, IDbTransaction transaction, ModelType obj)
        {
            bool policy = this.InitConnection(ref connection);

            List<IDbCommand> commands = this.InsertCommands(obj);

            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Connection = connection;
                if (transaction != null) commands[i].Transaction = transaction;

                this.ExecuteNonQuery(commands[i]);

                // If this is the first command, executing it
                // and getting last ID.
                // TODO: Not very good implementation, I think.
                if (i == 0)
                {
                    obj.Id = this.ReadLastId(connection, transaction);
                }
            }

            if (!policy) connection.Close();
        }

        /// <summary>
        /// Updates an object in database, then updates its version.
        /// </summary>
        /// <param name="connection">IDbConnection to use.</param>
        /// <param name="transaction">IDbTransaction to use. If null, no transaction will ne used.</param>
        /// <param name="obj">Object to update.</param>
        protected virtual void Update(IDbConnection connection, IDbTransaction transaction, ModelType obj)
        {
            bool policy = this.InitConnection(ref connection);
            List<IDbCommand> commands = this.UpdateCommands(obj);

            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Connection = connection;
                if (transaction != null) commands[i].Transaction = transaction;

                int result = this.ExecuteNonQuery(commands[i]);

                // If this is the first command, verifying for
                // number of rows affected.
                // TODO: Not very good implementation, though
                // it doesn't use string comparison.
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
            this.Delete(null, null, obj);
        }

        /// <summary>
        /// Deletes an object from database.
        /// </summary>
        /// <param name="connection">IDbConnection to use. If null, new connection will be created.</param>
        /// <param name="transaction">IDbTransaction to use. If null, no transaction will ne used.</param>
        /// <param name="obj">Object to delete.</param>
        public void Delete(IDbConnection connection, IDbTransaction transaction, IModel obj)
        {
            this.Delete(connection, transaction, obj as ModelType);
        }

        /// <summary>
        /// Deletes an object from database.
        /// </summary>
        /// <param name="connection">IDbConnection to use.</param>
        /// <param name="transaction">IDbTransaction to use. If null, no transaction will ne used.</param>
        /// <param name="obj">Object to delete.</param>
        public virtual void Delete(IDbConnection connection, IDbTransaction transaction, ModelType obj)
        {
            bool policy = this.InitConnection(ref connection);

            List<IDbCommand> commands = this.DeleteCommands(obj);

            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Connection = connection;
                if (transaction != null) commands[i].Transaction = transaction;

                int result = this.ExecuteNonQuery(commands[i]);

                // If this is the first command, verifying for
                // number of rows affected.
                // TODO: Not very good implementation, though
                // it doesn't use string comparison.
                if (i == 0 && result == 0)
                {
                    if (!policy) connection.Close();
                    throw new DBConcurrencyException();
                }
            }
            if (transaction == null) obj.Id = Id.Empty;
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
        /// Reads last inserted Id from database.
        /// </summary>
        /// <param name="connection">IDbConnection used for inserting the Id. Can not be null and should be opened.</param>
        /// <param name="transaction">IDbTransaction used for inserting the Id. Can be null.</param>
        /// <returns>Id instance.</returns>
        protected virtual Id ReadLastId(IDbConnection connection, IDbTransaction transaction)
        {
            IDbCommand command = this.LastIdCommand();
            command.Connection = connection;
            if (transaction != null) command.Transaction = transaction;

            return new Id(command.ExecuteScalar());
        }

        /// <summary>
        /// Executes an IDbCommand and returns the number of rows affected.
        /// </summary>
        /// <param name="command">IDbCommand to execute.</param>
        /// <returns>Number of rows affected.</returns>
        protected int ExecuteNonQuery(IDbCommand command)
        {
            IDbConnection connection = command.Connection;
            bool policy = this.InitConnection(ref connection);
            command.Connection = connection;

            int result = command.ExecuteNonQuery();

            if (!policy) connection.Close();
            return result;
        }

        /// <summary>
        /// Executes and IDbCommand and returns executed reader.
        /// </summary>
        /// <param name="command">IDbCommand to execute.</param>
        /// <returns>Number of rows affected.</returns>
        protected IDataReader ExecuteReader(IDbCommand command)
        {
            IDbConnection connection = command.Connection;
            bool policy = this.InitConnection(ref connection);
            command.Connection = connection;

            IDataReader result = command.ExecuteReader(policy ? 
                CommandBehavior.Default : 
                CommandBehavior.CloseConnection);
            return result;
        }

        /// <summary>
        /// Executes an IDbCommand and reads a single object.
        /// </summary>
        /// <param name="command">IDbCommand to execute.</param>
        /// <returns>Filled Model instance.</returns>
        protected ModelType Read(IDbCommand command)
        {
            ModelType result = default(ModelType);

            IDataReader reader = this.ExecuteReader(command);
            if (reader.Read()) result = this.ReadObject(reader);
            reader.Close();

            return result;
        }

        /// <summary>
        /// Executes an IDbCommand and reads all objects from the result.
        /// </summary>
        /// <param name="command">IDbCommand to execute.</param>
        /// <returns>List of Model instances.</returns>
        protected List<ModelType> ReadMultiple(IDbCommand command)
        {
            List<ModelType> result = new List<ModelType>();

            IDataReader reader = this.ExecuteReader(command);
            while (reader.Read()) result.Add(this.ReadObject(reader));
            reader.Close();

            return result;
        }

        /// <summary>
        /// Gets an IDbConnection.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        public abstract IDbConnection GetConnection();

        /// <summary>
        /// Initializes IDbConnection and returns behaviour
        /// policy.
        /// </summary>
        /// <param name="connection">Connection to initialize.</param>
        /// <returns>True if connection should stay open, overwise false.</returns>
        private bool InitConnection(ref IDbConnection connection)
        {
            // Initializing connection if needed.
            if (connection == null)
            {
                connection = this.GetConnection();
            }

            // Opening connection if needed.
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// When overridden in derived class, returns the query for reading a single object from database with specified Id.
        /// </summary>
        /// <param name="id">Object Id to read.</param>
        /// <returns>Initialized IDbCommand instance.</returns>
        protected abstract IDbCommand ReadCommand(Id id);

        /// <summary>
        /// When overridden in derived class, returns the query for reading all objects of current type from database.
        /// </summary>
        /// <returns>Initialized IDbCommand instance.</returns>
        protected abstract IDbCommand ReadAllCommand();

        /// <summary>
        /// When overridden in derived class, returns queries for writing new object to database.
        /// </summary>
        /// <param name="obj">Object to be inserted.</param>
        /// <returns>List of IDbCommand instances.</returns>
        protected abstract List<IDbCommand> InsertCommands(ModelType obj);

        /// <summary>
        /// When overridden in derived class, returns queries for updating an object in database.
        /// </summary>
        /// <param name="obj">Object to be updated.</param>
        /// <returns>List of IDbCommand instances.</returns>
        protected abstract List<IDbCommand> UpdateCommands(ModelType obj);

        /// <summary>
        /// When overridden in derived class, returns queries for deleting an object from database.
        /// </summary>
        /// <param name="obj">Object to be deleted.</param>
        /// <returns>List of IDbCommand instances.</returns>
        protected abstract List<IDbCommand> DeleteCommands(ModelType obj);

        /// <summary>
        /// Returns a query for reading the last inserted Id from database.
        /// </summary>
        /// <returns>Initialized IDbCommand instance.</returns>
        protected abstract IDbCommand LastIdCommand();
    }
}
