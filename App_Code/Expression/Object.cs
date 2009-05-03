using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Expression
{
    /// <summary>
    /// Represents Object expression.
    /// </summary>
    public class Object : IExpression
    {
        private object container;

        /// <summary>
        /// Gets Object expression container.
        /// </summary>
        public object Container
        {
            get { return this.container; }
        }

        public Object(object Object)
        {
            this.container = Object;
        }
    }
}
