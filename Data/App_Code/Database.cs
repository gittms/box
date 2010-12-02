using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Transactions;
using Definitif.Data.Queries;

namespace Definitif.Data
{
    /// <summary>
    /// Represents Database object.
    /// </summary>
    public abstract class Database : IEnumerable
    {
        protected Dictionary<string, Table> tables = new Dictionary<string, Table>();

        /// <summary>
        /// Gets database tables dictionary.
        /// </summary>
        public Dictionary<string, Table> Tables
        {
            get { return this.tables; }
        }
        /// <summary>
        /// Gets database name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets database state.
        /// </summary>
        public DatabaseState State
        {
            get { return this.state; }
        }
        protected DatabaseState state = DatabaseState.NotInitialized;

        /// <summary>
        /// Gets connection string.
        /// </summary>
        public string ConnectionString { get; internal set; }

        /// <summary>
        /// Gets Drawer object for Database.
        /// </summary>
        public Drawer Drawer { get; internal set; }

        /// <summary>
        /// Gets database NULL representation.
        /// </summary>
        public object NULL
        {
            get { return null; }
        }

        /// <summary>
        /// Initializes database instance by Connection String.
        /// </summary>
        /// <param name="connectionString">Connection String to use for initialization.</param>
        public virtual void Init(string connectionString)
        {
            this.ConnectionString = connectionString;
            this.Drawer = this.GetDrawer();
            this.UpdateSchema();
            this.state = DatabaseState.Initialized;
        }

        /// <summary>
        /// Gets Drawer object.
        /// </summary>
        /// <returns>Drawer object.</returns>
        protected abstract Drawer GetDrawer();
        /// <summary>
        /// Gets DbConnection object initialized.
        /// </summary>
        /// <returns>DbConnection object.</returns>
        protected abstract DbConnection GetDatabaseConnection();
        /// <summary>
        /// Gets DbCommand object without new connection initialized.
        /// </summary>
        /// <returns>DbCommand object.</returns>
        public abstract DbCommand GetCommand();
        /// <summary>
        /// Updates database schema.
        /// </summary>
        protected abstract void UpdateSchema();

        /// <summary>
        /// Gets DbConnection with transaction enlisted.
        /// </summary>
        /// <returns>DbConnection object.</returns>
        public DbConnection GetConnection()
        {
            DbConnection connection = this.GetDatabaseConnection();

            if (transaction != null)
            {
                // Enlisting transaction requires open connection.
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                connection.EnlistTransaction(transaction);
            }

            return connection;
        }

        /// <summary>
        /// Gets Command object for given Connection with given Command text.
        /// </summary>
        /// <param name="connection">Connection object.</param>
        /// <param name="CommandText">Command text.</param>
        /// <returns>Command object.</returns>
        protected DbCommand GetCommand(DbConnection connection, string commandText)
        {
            if (connection.State != ConnectionState.Open) connection.Open();
            DbCommand command = this.GetCommand();
            command.Connection = connection;
            command.CommandText = commandText;
            return command;
        }
        /// <summary>
        /// Gets Command object for given Connection.
        /// </summary>
        /// <param name="connection">Connection object.</param>
        /// <returns>Command object.</returns>
        protected DbCommand GetCommand(DbConnection connection)
        {
            return this.GetCommand(connection, "");
        }
        /// <summary>
        /// Gets Command object for given Query.
        /// Does not assotiates connection.
        /// </summary>
        /// <param name="query">Query object.</param>
        /// <returns>Command object.</returns>
        public DbCommand GetCommand(Query query)
        {
            DbCommand command = this.GetCommand();
            command.CommandText = query.ToString(this.Drawer);
            return command;
        }
        /// <summary>
        /// Gets Command object for given Query.
        /// </summary>
        /// <param name="query">Query object.</param>
        /// <returns>Command object.</returns>
        public DbCommand GetCommand(Query query, bool initConnection)
        {
            if (initConnection)
            {
                DbCommand command = this.GetCommand(this.GetConnection(), query.ToString(this.Drawer));
                return command;
            }
            else
            {
                return this.GetCommand(query);
            }
        }
        /// <summary>
        /// Gets Command object with given Command text.
        /// </summary>
        /// <param name="commandText">Command text.</param>
        /// <returns>Command object.</returns>
        protected DbCommand GetCommand(string commandText)
        {
            return this.GetCommand(this.GetConnection(), commandText);
        }
        /// <summary>
        /// Gets DataReader object for given Command.
        /// </summary>
        /// <param name="command">Command object.</param>
        /// <param name="closeConnection">If True, Connection will be closed after execution.</param>
        /// <returns>DataReader object.</returns>
        protected IDataReader GetDataReader(DbCommand command, bool closeConnection)
        {
            return closeConnection ?
                command.ExecuteReader(CommandBehavior.CloseConnection) :
                command.ExecuteReader();
        }
        /// <summary>
        /// Gets DataReader object for given Command with policy to close connection.
        /// </summary>
        /// <param name="command">Command object.</param>
        /// <returns>DataReader object.</returns>
        protected IDataReader GetDataReader(DbCommand command)
        {
            return this.GetDataReader(command, true);
        }
        /// <summary>
        /// Gets DataReader object for new Command with given Command text and policy to close connection.
        /// </summary>
        /// <param name="commandText">Command text.</param>
        /// <returns>DataReader object.</returns>
        protected IDataReader GetDataReader(string commandText)
        {
            return this.GetDataReader(this.GetCommand(commandText));
        }

        /// <summary>
        /// Executes non-query command and returns number of rows
        /// affected by query.
        /// </summary>
        /// <param name="queries">Qeries objects to Execute.</param>
        /// <returns>Number of rows affected.</returns>
        public virtual int Execute(params Query[] queries)
        {
            return this.Execute(false, queries);
        }
        public virtual int Execute(params string[] queries)
        {
            int result = 0;
            DbCommand command;
            DbConnection connection = this.GetConnection();

            foreach (string query in queries)
            {
                command = this.GetCommand(connection, query);
                result += command.ExecuteNonQuery();
            }

            connection.Close();
            return result;
        }
        /// <summary>
        /// Executes non-query command and returns number of rows
        /// affected by query or last query identity.
        /// </summary>
        /// <param name="getIdentity">If "True", identity will be returned.</param>
        /// <param name="queries">Qeries objects to Execute.</param>
        /// <returns>Number of rows affected.</returns>
        public virtual int Execute(bool getIdentity, params Query[] queries)
        {
            int result = 0;
            DbCommand command;
            DbConnection connection = this.GetConnection();

            foreach (Query query in queries)
            {
                command = this.GetCommand(connection, query.ToString(this.Drawer));
                result += command.ExecuteNonQuery();
            }

            // Getting identity for last executed query.
            if (getIdentity)
            {
                command = this.GetCommand(connection, this.Drawer.Identity);
                result = (int?)command.ExecuteScalar() ?? 0;
            }

            connection.Close();
            return result;
        }

        /// <summary>
        /// Executes readable command and returns executed
        /// reader object.
        /// </summary>
        /// <param name="query">Query object to execute.</param>
        /// <returns>Executed reader object.</returns>
        public virtual IDataReader ExecuteReader(Query query)
        {
            return this.GetDataReader(query.ToString(this.Drawer));
        }

        #region Transaction management.

        // CommittableTransaction implemented as ThreadStatic to
        // implement thread-safe transactions.
        // NOTE:
        //   Such implementation does not allow to perform transactions
        //   on more than one database at a time.

        /// <summary>
        /// Currently active committable transaction.
        /// </summary>
        [ThreadStatic]
        protected static CommittableTransaction transaction;

        /// <summary>
        /// Starts new transaction using options given.
        /// </summary>
        /// <param name="options">Options to use for transaction creation.</param>
        /// <exception cref="TransactionException" />
        public void TransactionBegin(TransactionOptions options)
        {
            if (transaction != null)
            {
                throw new TransactionException("Active transaction already exists.");
            }
            transaction = new CommittableTransaction(options);
        }

        /// <summary>
        /// Starts new transaction.
        /// </summary>
        /// <exception cref="TransactionException" />
        public void TransactionBegin()
        {
            TransactionBegin(new TransactionOptions());
        }

        /// <summary>
        /// Starts new transaction using isolation level given.
        /// </summary>
        /// <param name="isolationLevel">Isolation level to use.</param>
        /// <exception cref="TransactionException" />
        public void TransactionBegin(System.Transactions.IsolationLevel isolationLevel)
        {
            TransactionBegin(new TransactionOptions()
            {
                IsolationLevel = isolationLevel,
            });
        }

        /// <summary>
        /// Checks if currently active transaction exists.
        /// </summary>
        public bool TransactionIsActive
        {
            get { return transaction != null; }
        }

        /// <summary>
        /// Commits current active transaction
        /// </summary>
        /// <exception cref="TransactionException" />
        public void TransactionCommit()
        {
            if (transaction == null)
            {
                throw new TransactionException("Active transaction does not exist.");
            }
            transaction.Commit();

            transaction.Dispose();
            transaction = null;
        }

        /// <summary>
        /// Rolls back current active transaction.
        /// </summary>
        /// <exception cref="TransactionException" />
        public void TransactionRollback()
        {
            if (transaction == null)
            {
                throw new TransactionException("Active transaction does not exist.");
            }
            transaction.Rollback();

            transaction.Dispose();
            transaction = null;
        }

        #endregion

        /// <summary>
        /// Gets database table object by name.
        /// </summary>
        /// <param name="name">Name of table to get.</param>
        /// <returns>Requested table object.</returns>
        public Table this[string name]
        {
            get
            {
                Table table;
                if (this.tables.TryGetValue(name, out table))
                {
                    return table;
                }
                else
                {
                    throw new IndexOutOfRangeException(String.Format(
                        "Database '{0}' does not contain definition for table '{1}'.",
                        this.Name, name));
                }
            }
        }

        /// <summary>
        /// Adds specified Table object to Database.
        /// </summary>
        /// <param name="table">Table object to add.</param>
        internal void Add(Table table)
        {
            table.Database = this;
            this.tables.Add(table.Name, table);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.tables.Values.GetEnumerator();
        }

        /// <summary>
        /// Creates specified Table object in Database.
        /// </summary>
        /// <param name="table">Table object to create.</param>
        public void CreateTable(Table table)
        {
            this.Execute(this.Drawer.DrawTableCreate(table));
            this.Add(table);
        }
        /// <summary>
        /// Drops specified Table object from Database.
        /// </summary>
        /// <param name="table">Table object to drop.</param>
        public void DropTable(Table table)
        {
            this.Execute(this.Drawer.DrawTableDrop(table));
            this.tables.Remove(table.Name);
        }
    }
}
