using System;
using System.Data;

namespace Definitif.Data.CommonBox
{
    /// <summary>
    /// IModel object associated with a unit of work action type.
    /// </summary>
    internal class UoWAction
    {
        public ActionType Type;
        public IModel Object;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="type">Unit of work action type.</param>
        /// <param name="obj">IModel object.</param>
        public UoWAction(ActionType type, IModel obj)
        {
            this.Type = type;
            this.Object = obj;
        }

        /// <summary>
        /// Adds unit of work action to transaction.
        /// </summary>
        internal void Transact(ref IDbConnection connection, ref IDbTransaction transaction)
        {
            // Initializing connection and transaction.
            if (connection == null)
            {
                connection = this.Object.IMapper().GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();
            }

            if (this.Type == ActionType.Write)
            {
                this.Object.IMapper().Write(connection, transaction, this.Object);
            }
            else if (this.Type == ActionType.Delete)
            {
                this.Object.IMapper().Delete(connection, transaction, this.Object);
            }
        }
    }
}
