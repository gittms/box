using System;
using System.Configuration;
using System.Reflection;
using Definitif.Data;

namespace Definitif
{
    /// <summary>
    /// Represents core of Definitif Box Library.
    /// </summary>
    public class Core
    {
        protected static Core core;

        /// <summary>
        /// Gets core database object.
        /// </summary>
        public static Database Database
        {
            get
            {
                return core.database;
            }
        }

        /// <summary>
        /// Gets database object.
        /// </summary>
        protected Database database { get; set; }

        /// <summary>
        /// Creates an instance of core.
        /// </summary>
        public Core() : this(null) { }

        /// <summary>
        /// Creates an instance of core.
        /// </summary>
        public Core(Type coreType)
        {
            // Getting connection strings.
            ConnectionStringSettingsCollection connections = ConfigurationManager.ConnectionStrings;

            if (connections.Count == 0)
            {
                throw new ConfigurationErrorsException("Appication configuration does not contain connection strings.");
            }
            else
            {
                string key = "Core";
                if (coreType != null)
                {
                    foreach (object attribute in coreType.GetCustomAttributes(typeof(ConnectionStringKeyAttribute), false))
                    {
                        key = (attribute as ConnectionStringKeyAttribute).Key;
                    }
                }

                // Getting connection string by key provided.
                ConnectionStringSettings connectionString;
                try
                {
                    connectionString = connections[key];
                }
                catch
                {
                    throw new ConfigurationErrorsException(String.Format("Application configuration does not contain '{0}' connection string.", key));
                }

                // Trying to create provider.
                Database database;
                try
                {
                    object provider = Activator.CreateInstance(Type.GetType(connectionString.ProviderName));
                    if (!(provider is Database))
                    {
                        throw new ConfigurationErrorsException(String.Format("Provider '{0}' is not implementation of Definitif.Data.Database."));
                    }
                    database = provider as Database;
                }
                catch
                {
                    throw new ConfigurationErrorsException(String.Format("Provider '{0}' can not be activated.", connectionString.ProviderName));
                }

                // Initializing database.
                database.Init(connectionString.ConnectionString);
            }
        }
    }
}
