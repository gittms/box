using System;
using System.Collections.Generic;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.ObjectSql.Expression
{
    /// <summary>
    /// Represents LIKE expression:
    /// [IExpression] LIKE [string]
    /// </summary>
    public class LIKE : IExpression
    {
        private string expression;
        private IColumn[] container;

        /// <summary>
        /// Gets column container.
        /// </summary>
        public IColumn[] Container
        {
            get { return this.container; }
        }

        /// <summary>
        /// Gets string LIKE expression.
        /// </summary>
        public string Expression
        {
            get { return this.expression; }
        }

        public LIKE(
            string Expression,
            IColumn Column)
        {
            this.container = new IColumn[] { Column };
            this.expression = Expression;
        }
    }
}
