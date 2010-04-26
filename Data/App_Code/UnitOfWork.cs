using System;
using System.Collections.Generic;
using System.Data;

namespace Definitif.Data
{
    /// <summary>
    /// Provides methods for writing or deleting a number of objects using a database transaction.
    /// </summary>
    public class UnitOfWork
    {
        private List<UoWAction> actions = new List<UoWAction>();

        /// <summary>
        /// Creates new instance.
        /// </summary>
        public UnitOfWork()
        { }

        /// <summary>
        /// Adds an IModel object for writing.
        /// </summary>
        /// <param name="obj">IModel object to write.</param>
        public void Write(IModel obj)
        {
            this.actions.Add(new UoWAction(ActionType.Write, obj));
        }

        /// <summary>
        /// Adds an IModel object for deleting.
        /// </summary>
        /// <param name="obj">IModel object to delete.</param>
        public void Delete(IModel obj)
        {
            this.actions.Add(new UoWAction(ActionType.Delete, obj));
        }

        /// <summary>
        /// Performs all pending actions using a database transaction.
        /// The transaction is rolled back if an error occurs.
        /// </summary>
        public void Commit()
        {
            if (this.actions.Count == 0) return;

            IDbConnection connection = null; IDbTransaction transaction = null;
            try
            {
                foreach (UoWAction action in this.actions)
                {
                    action.Transact(ref connection, ref transaction);
                }
                transaction.Commit();
                connection.Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                connection.Close();
                throw ex;
            }
        }
    }
}
