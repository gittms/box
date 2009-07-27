using System;
using System.Collections;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents common abstract sql
    /// queries drawer.
    /// </summary>
    public abstract partial class Drawer
    {
        /// <summary>
        /// Throws exception about non implemented drawer.
        /// </summary>
        /// <param name="Object">Object drawer does not support.</param>
        private string Except(object Object)
        {
            #if DEBUG
            return " [UNKNOWN] ";
            #endif
            throw new NotImplementedException(
                String.Format(
                    "Handling of '{0}' is not implemented in Drawer.Draw().",
                    Object.GetType().ToString()
                ));
        }

        /// <summary>
        /// Converts list of ITable objects to comma-separated list representation.
        /// </summary>
        /// <param name="List">List of ITable objects.</param>
        protected string CommaSeparated(IList<ITable> List)
        {
            string result = "";

            for (int i = 0; i < List.Count; i++)
            {
                if (i > 0) result += ", ";
                result += this.Draw(List[i] as ITable);
            }

            return result;
        }

        /// <summary>
        /// Converts list of IColumn objects to comma-separated list representation.
        /// </summary>
        /// <param name="List">List of IColumn objects.</param>
        protected string CommaSeparated(IList<IColumn> List)
        {
            string result = "";

            for (int i = 0; i < List.Count; i++)
            {
                if (i > 0) result += ", ";
                result += this.Draw(List[i] as IColumn);
            }

            return result;
        }

        /// <summary>
        /// Converts list of IExpression objects to comma-separated list representation.
        /// </summary>
        /// <param name="List">List of IExpression objects.</param>
        protected string CommaSeparated(IList<IExpression> List)
        {
            string result = "";

            for (int i = 0; i < List.Count; i++)
            {
                if (i > 0) result += ", ";
                result += this.Draw(List[i] as IExpression);
            }

            return result;
        }
    }
}
