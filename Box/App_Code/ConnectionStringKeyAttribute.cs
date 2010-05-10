using System;

namespace Definitif
{
    /// <summary>
    /// Specifies configuration connection string key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConnectionStringKeyAttribute : Attribute
    {
        public string Key { get; internal set; }

        /// <summary>
        /// Specifies configuration connection string key.
        /// </summary>
        /// <param name="key">Key to get connection string by.</param>
        public ConnectionStringKeyAttribute(string key)
        {
            this.Key = key;
        }
    }
}
