using System;
using System.Collections.Generic;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.ObjectSql.Expression
{
    /// <summary>
    /// Represents CONTAINS expression.
    /// </summary>
    public class CONTAINS : IExpression
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
        /// Gets string CONTAINS expression.
        /// </summary>
        public string Expression
        {
            get { return this.expression; }
        }

        public CONTAINS(
            string Expression,
            params IColumn[] Column)
        {
            this.container = Column;
            this.expression = Expression;
        }
    }
}
