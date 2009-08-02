using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents possible database statuses.
    /// </summary>
    public enum DatabaseState
    {
        /// <summary>
        /// Database is initialized.
        /// </summary>
        Initialized,
        /// <summary>
        /// Database is not yet initialized.
        /// </summary>
        NotInitialized
    }

    /// <summary>
    /// Represents Database object.
    /// </summary>
    public abstract class Database : IEnumerable
    {
        protected Dictionary<string, Table> tables
            = new Dictionary<string, Table>();
        protected DatabaseState state
            = DatabaseState.NotInitialized;
        protected string name;
        protected string connectionString;
        protected Drawer drawer;

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
        public string Name
        {
            get { return this.name; }
        }
        /// <summary>
        /// Gets database state.
        /// </summary>
        public DatabaseState State
        {
            get { return this.state; }
        }
        /// <summary>
        /// Gets connection string.
        /// </summary>
        public string ConnectionString
        {
            get { return this.connectionString; }
        }

        /// <summary>
        /// Initializes database instance by Connection String.
        /// </summary>
        /// <param name="ConnectionString">Connection String to use for initialization.</param>
        public virtual void Init(string ConnectionString)
        {
            this.connectionString = ConnectionString;
            this.drawer = this.GetDrawer();
            this.UpdateSchema();
            this.state = DatabaseState.Initialized;
        }

        /// <summary>
        /// Gets Drawer object for Database.
        /// </summary>
        public Drawer Drawer
        {
            get { return this.drawer; }
        }

        /// <summary>
        /// Gets Drawer object.
        /// </summary>
        /// <returns>Drawer object.</returns>
        protected abstract Drawer GetDrawer();
        /// <summary>
        /// Gets Connection object initialized.
        /// </summary>
        /// <returns>Connection object.</returns>
        protected abstract IDbConnection GetConnection();
        /// <summary>
        /// Gets Command object for given Connection with given Command text.
        /// </summary>
        /// <param name="Connection">Connection object.</param>
        /// <param name="CommandText">Command text.</param>
        /// <returns>Command object.</returns>
        protected abstract IDbCommand GetCommand(IDbConnection Connection, string CommandText);
        /// <summary>
        /// Gets Command object with new Connection initialized.
        /// </summary>
        /// <returns>Command object.</returns>
        protected IDbCommand GetCommand()
        {
            return this.GetCommand(
                this.GetConnection(), "");
        }
        /// <summary>
        /// Gets Command object for given Connection.
        /// </summary>
        /// <param name="Connection">Connection object.</param>
        /// <returns>Command object.</returns>
        protected IDbCommand GetCommand(IDbConnection Connection)
        {
            return this.GetCommand(
                Connection, "");
        }
        /// <summary>
        /// Gets Command object with given Command text.
        /// </summary>
        /// <param name="CommandText">Command text.</param>
        /// <returns>Command object.</returns>
        protected IDbCommand GetCommand(string CommandText)
        {
            return this.GetCommand(
                this.GetConnection(), CommandText);
        }
        /// <summary>
        /// Gets DataReader object for given Command.
        /// </summary>
        /// <param name="Command">Command object.</param>
        /// <param name="CloseConnection">If True, Connection will be closed after execution.</param>
        /// <returns>DataReader object.</returns>
        protected IDataReader GetDataReader(IDbCommand Command, bool CloseConnection)
        {
            return CloseConnection ?
                Command.ExecuteReader(CommandBehavior.CloseConnection) :
                Command.ExecuteReader();
        }
        /// <summary>
        /// Gets DataReader object for given Command with policy to close connection.
        /// </summary>
        /// <param name="Command">Command object.</param>
        /// <returns>DataReader object.</returns>
        protected IDataReader GetDataReader(IDbCommand Command)
        {
            return this.GetDataReader(Command, true);
        }
        /// <summary>
        /// Gets DataReader object for new Command with given Command text and policy to close connection.
        /// </summary>
        /// <param name="CommandText">Command text.</param>
        /// <returns>DataReader object.</returns>
        protected IDataReader GetDataReader(string CommandText)
        {
            return this.GetDataReader(
                this.GetCommand(CommandText));
        }

        /// <summary>
        /// Updates database schema.
        /// </summary>
        protected abstract void UpdateSchema();

        /// <summary>
        /// Executes non-query command and returns number of rows
        /// affected by query.
        /// </summary>
        /// <param name="Queries">Qeries objects to Execute.</param>
        /// <returns>Number of rows affected.</returns>
        public virtual int Execute(params IQuery[] Queries)
        {
            int result = 0;
            IDbCommand command;
            IDbConnection connection = this.GetConnection();

            foreach (IQuery query in Queries)
            {
                command = this.GetCommand(connection, this.drawer.Draw(query));
                result += command.ExecuteNonQuery();
            }

            connection.Close();
            return result;
        }
        /// <summary>
        /// Executes readable command and returns executed
        /// reader object.
        /// </summary>
        /// <param name="Query">Query object to execute.</param>
        /// <returns>Executed reader object.</returns>
        public virtual IDataReader ExecuteReader(IQuery Query)
        {
            return this.GetDataReader(this.drawer.Draw(Query));
        }

        /// <summary>
        /// Gets database table object by name.
        /// </summary>
        /// <param name="Table">Name of table to get.</param>
        /// <returns>Requested table object.</returns>
        public Table this[string Table]
        {
            get
            {
                if (this.tables.ContainsKey(Table))
                {
                    return this.tables[Table];
                }
                else
                {
                    throw new ObjectSqlException(
                        String.Format(
                            "Database '{0}' does not contain definition for table '{1}'.",
                            this.name, Table
                        ));
                }
            }
        }

        /// <summary>
        /// Adds specified Table object to Database.
        /// </summary>
        /// <param name="Table">Table object to add.</param>
        public void Add(Table Table)
        {
            Table.Database = this;
            this.tables.Add(Table.Name, Table);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.tables.Values.GetEnumerator();
        }
    }
}
