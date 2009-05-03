using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Join
{
    // AUTODOC: class Join.Join
    public class Join : ITable, IJoinable
    {
        protected IJoinable first, second;
        protected List<IExpression> on
            = new List<IExpression>();

        /// <summary>
        /// Gets or sets table join applies by. 
        /// </summary>
        public IJoinable First
        {
            get { return this.first; }
            set { this.first = value; }
        }
        /// <summary>
        /// Gets or sets table join applies to.
        /// </summary>
        public IJoinable Second
        {
            get { return this.second; }
            set { this.second = value; }
        }
        /// <summary>
        /// Gets list of expressions that applied to join.
        /// </summary>
        public List<IExpression> ON
        {
            get { return this.on; }
        }

        [Obsolete("Property Join.Name implemented for interface compatibility only.")]
        public string Name
        {
            get { return ""; }
        }

        /// <summary>
        /// Gets table columns dictionary.
        /// </summary>
        public Dictionary<string, Column> Columns
        {
            get
            {
                Dictionary<string, Column> columns =
                    new Dictionary<string, Column>();

                foreach (Column column in this.first.Columns.Values)
                {
                    columns.Add(column.Name, column);
                }
                foreach (Column column in this.second.Columns.Values)
                {
                    columns.Add(column.Name, column);
                }

                return columns;
            }
        }

        /// <summary>
        /// Gets table column object by name.
        /// </summary>
        /// <param name="Column">Name of column to get.</param>
        /// <returns>Requested column object.</returns>
        public Column this[string Column]
        {
            get
            {
                if (this.first.Columns.ContainsKey(Column))
                    return this.first[Column];
                else if (this.second.Columns.ContainsKey(Column))
                    return this.second[Column];
                else
                    throw new ObjectSqlException(
                        String.Format(
                            "Nither table '{0}', nor '{1}' does not contain definition for column '{2}'.",
                            this.first.Name, this.second.Name, Column
                        ));
            }
        }

        public Join()
        { }

        // AUTODOC: constructor Join.Join
        public Join(IJoinable First, IJoinable Second, params IExpression[] On)
        {
            this.first = First;
            this.second = Second;
            foreach (IExpression expression in On)
            {
                this.on.Add(expression);
            }
        }
    }
}
