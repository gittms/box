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
        /// <param name="Id">Model Id.</param>
        /// <returns>Model instance.</returns>
        public ModelType Read(Id Id)
        {
            return this.Read(null, Id);
        }

        /// <summary>
        /// Reads an object with specified Id from database.
        /// </summary>
        /// <param name="Connection">IDbConnection to use. If null, new connection will be created.</param>
        /// <param name="Id">Model Id.</param>
        /// <returns>Model instance.</returns>
        public virtual ModelType Read(IDbConnection Connection, Id Id)
        {
            bool policy = this.InitConnection(ref Connection);
            IDbCommand command = this.ReadCommand(Id);
            command.Connection = Connection;

            ModelType result = this.Read(command);
            if (!policy) Connection.Close();
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
        /// <param name="Connection">IDbConnection to use. If null, new connection will be created.</param>
        /// <returns>List of Model objects.</returns>
        public virtual List<ModelType> ReadAll(IDbConnection Connection)
        {
            bool policy = this.InitConnection(ref Connection);
            IDbCommand command = this.ReadAllCommand();
            command.Connection = Connection;

            List<ModelType> result = this.ReadMultiple(command);
            if (!policy) Connection.Close();
            return result;
        }

        /// <summary>
        /// Writes an object to database.
        /// </summary>
        /// <param name="Object">Object to write.</param>
        public void Write(IModel Object)
        {
            this.Write(Object as ModelType);
        }

        /// <summary>
        /// Writes an object to database.
        /// </summary>
        /// <param name="Object">Object to write.</param>
        public virtual void Write(ModelType Object)
        {
            this.Write(null, null, Object);
        }

        /// <summary>
        /// Writes an object to database.
        /// </summary>
        /// <param name="Connection">IDbConnection to use. If null, new connection will be created.</param>
        /// <param name="Transaction">IDbTransaction to use. If null, no transaction will ne used.</param>
        /// <param name="Object">Object to write.</param>
        public void Write(IDbConnection Connection, IDbTransaction Transaction, IModel Object)
        {
            this.Write(Connection, Transaction, Object as ModelType);
        }

        /// <summary>
        /// Writes an object to database.
        /// </summary>
        /// <param name="Connection">IDbConnection to use. If null, new connection will be created.</param>
        /// <param name="Transaction">IDbTransaction to use. If null, no transaction will ne used.</param>
        /// <param name="Object">Object to write.</param>
        public virtual void Write(IDbConnection Connection, IDbTransaction Transaction, ModelType Object)
        {
            bool policy = this.InitConnection(ref Connection);
            if (Object.Id.Equals(Id.Empty)) this.Insert(Connection, Transaction, Object);
            else this.Update(Connection, Transaction, Object);

            if (!policy) Connection.Close();
        }

        /// <summary>
        /// Inserts an object to database, then updates its Id.
        /// </summary>
        /// <param name="Connection">IDbConnection to use.</param>
        /// <param name="Transaction">IDbTransaction to use. If null, no transaction will ne used.</param>
        /// <param name="Object">Object to insert.</param>
        protected virtual void Insert(IDbConnection Connection, IDbTransaction Transaction, ModelType Object)
        {
            bool policy = this.InitConnection(ref Connection);

            List<IDbCommand> commands = this.InsertCommands(Object);

            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Connection = Connection;
                if (Transaction != null) commands[i].Transaction = Transaction;

                this.ExecuteNonQuery(commands[i]);

                // If this is the first command, executing it
                // and getting last ID.
                // TODO: Not very good implementation, I think.
                if (i == 0)
                {
                    Object.Id = this.ReadLastId(Connection, Transaction);
                }
            }

            if (!policy) Connection.Close();
        }

        /// <summary>
        /// Updates an object in database, then updates its version.
        /// </summary>
        /// <param name="Connection">IDbConnection to use.</param>
        /// <param name="Transaction">IDbTransaction to use. If null, no transaction will ne used.</param>
        /// <param name="Object">Object to update.</param>
        protected virtual void Update(IDbConnection Connection, IDbTransaction Transaction, ModelType Object)
        {
            bool policy = this.InitConnection(ref Connection);
            List<IDbCommand> commands = this.UpdateCommands(Object);

            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Connection = Connection;
                if (Transaction != null) commands[i].Transaction = Transaction;

                int result = this.ExecuteNonQuery(commands[i]);

                // If this is the first command, verifying for
                // number of rows affected.
                // TODO: Not very good implementation, though
                // it doesn't use string comparison.
                if (i == 0 && result == 0)
                {
                    throw new DBConcurrencyException();
                }
            }

            if (!policy) Connection.Close();
        }

        /// <summary>
        /// Deletes an object from database.
        /// </summary>
        /// <param name="Object">Object to delete.</param>
        public void Delete(IModel Object)
        {
            this.Delete(Object as ModelType);
        }

        /// <summary>
        /// Deletes an object from database.
        /// </summary>
        /// <param name="Object">Object to delete.</param>
        public void Delete(ModelType Object)
        {
            this.Delete(null, null, Object);
        }

        /// <summary>
        /// Deletes an object from database.
        /// </summary>
        /// <param name="Connection">IDbConnection to use. If null, new connection will be created.</param>
        /// <param name="Transaction">IDbTransaction to use. If null, no transaction will ne used.</param>
        /// <param name="Object">Object to delete.</param>
        public void Delete(IDbConnection Connection, IDbTransaction Transaction, IModel Object)
        {
            this.Delete(Connection, Transaction, Object as ModelType);
        }

        /// <summary>
        /// Deletes an object from database.
        /// </summary>
        /// <param name="Connection">IDbConnection to use.</param>
        /// <param name="Transaction">IDbTransaction to use. If null, no transaction will ne used.</param>
        /// <param name="Object">Object to delete.</param>
        public virtual void Delete(IDbConnection Connection, IDbTransaction Transaction, ModelType Object)
        {
            bool policy = this.InitConnection(ref Connection);

            List<IDbCommand> commands = this.DeleteCommands(Object);

            for (int i = 0; i < commands.Count; i++)
            {
                commands[i].Connection = Connection;
                if (Transaction != null) commands[i].Transaction = Transaction;

                int result = this.ExecuteNonQuery(commands[i]);

                // If this is the first command, verifying for
                // number of rows affected.
                // TODO: Not very good implementation, though
                // it doesn't use string comparison.
                if (i == 0 && result == 0)
                {
                    throw new DBConcurrencyException();
                }
            }
            if (Transaction == null) Object.Id = Id.Empty;
            Object.Version = 0;

            if (!policy) Connection.Close();
        }

        /// <summary>
        /// When overridden in derived class, returns a Model filled from executed IDataReader.
        /// </summary>
        /// <param name="dr">Executed IDataReader.</param>
        /// <returns>Filled Model instance.</returns>
        protected abstract ModelType Read(IDataReader dr);

        /// <summary>
        /// Reads last inserted Id from database.
        /// </summary>
        /// <param name="Connection">IDbConnection used for inserting the Id. Can not be null and should be opened.</param>
        /// <param name="Transaction">IDbTransaction used for inserting the Id. Can be null.</param>
        /// <returns>Id instance.</returns>
        protected virtual Id ReadLastId(IDbConnection Connection, IDbTransaction Transaction)
        {
            IDbCommand command = this.LastIdCommand();
            command.Connection = Connection;
            if (Transaction != null) command.Transaction = Transaction;

            return new Id(command.ExecuteScalar());
        }

        /// <summary>
        /// Executes an IDbCommand and returns the number of rows affected.
        /// </summary>
        /// <param name="Command">IDbCommand to execute.</param>
        /// <returns>Number of rows affected.</returns>
        protected int ExecuteNonQuery(IDbCommand Command)
        {
            IDbConnection connection = Command.Connection;
            bool policy = this.InitConnection(ref connection);
            Command.Connection = connection;

            int result = Command.ExecuteNonQuery();

            if (!policy) connection.Close();
            return result;
        }

        /// <summary>
        /// Executes and IDbCommand and returns executed reader.
        /// </summary>
        /// <param name="Command">IDbCommand to execute.</param>
        /// <returns>Number of rows affected.</returns>
        protected IDataReader ExecuteReader(IDbCommand Command)
        {
            IDbConnection connection = Command.Connection;
            bool policy = this.InitConnection(ref connection);
            Command.Connection = connection;

            IDataReader result = Command.ExecuteReader(policy ? 
                CommandBehavior.Default : 
                CommandBehavior.CloseConnection);
            return result;
        }

        /// <summary>
        /// Executes an IDbCommand and reads a single object.
        /// </summary>
        /// <param name="Command">IDbCommand to execute.</param>
        /// <returns>Filled Model instance.</returns>
        protected ModelType Read(IDbCommand Command)
        {
            ModelType result = default(ModelType);

            IDataReader reader = this.ExecuteReader(Command);
            if (reader.Read()) result = this.Read(reader);
            reader.Close();

            return result;
        }

        /// <summary>
        /// Executes an IDbCommand and reads all objects from the result.
        /// </summary>
        /// <param name="Command">IDbCommand to execute.</param>
        /// <returns>List of Model instances.</returns>
        protected List<ModelType> ReadMultiple(IDbCommand Command)
        {
            List<ModelType> result = new List<ModelType>();

            IDataReader reader = this.ExecuteReader(Command);
            while (reader.Read()) result.Add(this.Read(reader));
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
        /// <param name="Connection">Connection to initialize.</param>
        /// <returns>True if connection should stay open, overwise false.</returns>
        private bool InitConnection(ref IDbConnection Connection)
        {
            // Initializing connection if needed.
            if (Connection == null)
            {
                Connection = this.GetConnection();
            }

            // Opening connection if needed.
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
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
        /// <param name="Id">Object Id to read.</param>
        /// <returns>Initialized IDbCommand instance.</returns>
        protected abstract IDbCommand ReadCommand(Id Id);

        /// <summary>
        /// When overridden in derived class, returns the query for reading all objects of current type from database.
        /// </summary>
        /// <returns>Initialized IDbCommand instance.</returns>
        protected abstract IDbCommand ReadAllCommand();

        /// <summary>
        /// When overridden in derived class, returns queries for writing new object to database.
        /// </summary>
        /// <param name="Object">Object to be inserted.</param>
        /// <returns>List of IDbCommand instances.</returns>
        protected abstract List<IDbCommand> InsertCommands(ModelType Object);

        /// <summary>
        /// When overridden in derived class, returns queries for updating an object in database.
        /// </summary>
        /// <param name="Object">Object to be updated.</param>
        /// <returns>List of IDbCommand instances.</returns>
        protected abstract List<IDbCommand> UpdateCommands(ModelType Object);

        /// <summary>
        /// When overridden in derived class, returns queries for deleting an object from database.
        /// </summary>
        /// <param name="Object">Object to be deleted.</param>
        /// <returns>List of IDbCommand instances.</returns>
        protected abstract List<IDbCommand> DeleteCommands(ModelType Object);

        /// <summary>
        /// Returns a query for reading the last inserted Id from database.
        /// </summary>
        /// <returns>Initialized IDbCommand instance.</returns>
        protected abstract IDbCommand LastIdCommand();
    }
}
