using System;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents trowable ObjectSql exception.
    /// </summary>
    public class ObjectSqlException : Exception
    {
        private string message;

        /// <summary>
        /// Gets expection message.
        /// </summary>
        public override string Message
        {
            get { return this.message; }
        }

        /// <summary>
        /// Creates filled instance of throwable
        /// ObjectSql exception.
        /// </summary>
        /// <param name="Message"></param>
        public ObjectSqlException(string Message)
        {
            this.message = Message;
        }
    }
}
