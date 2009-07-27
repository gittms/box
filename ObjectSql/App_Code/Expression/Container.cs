using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Expression
{
    /// <summary>
    /// Represents single-container expression
    /// object.
    /// </summary>
    public abstract class SingleContainer : Expression
    {
        protected IExpression[] container;

        /// <summary>
        /// Gets content container.
        /// </summary>
        public IExpression[] Container
        {
            get { return this.container; }
        }
    }

    /// <summary>
    /// Represents double-container expression
    /// object.
    /// </summary>
    public abstract class DoubleContainer : Expression
    {
        protected IExpression[] first, second;

        /// <summary>
        /// Gets first content container.
        /// </summary>
        public IExpression[] FirstContainer
        {
            get { return this.first; }
        }

        /// <summary>
        /// Gets second content container.
        /// </summary>
        public IExpression[] SecondContainer
        {
            get { return this.second; }
        }
    }
}
