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

        public SingleContainer()
        { }

        public SingleContainer(params IExpression[] Expressions)
        {
            this.container = Expressions;
        }

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
        protected IExpression[] firstContainer, secondContainer;

        /// <summary>
        /// Creates double container expression object.
        /// </summary>
        public DoubleContainer()
        { }

        public DoubleContainer(IExpression[] firstContainer, IExpression[] secondContainer)
            : this(firstContainer, secondContainer, true) { }

        public DoubleContainer(IExpression[] firstContainer, IExpression[] secondContainer, bool verifySecondLength)
        {
            if (firstContainer.Length != 1 || (verifySecondLength && secondContainer.Length != 1))
            {
                throw new ObjectSqlException(
                    this.GetType().Name.ToString() + " should contain single expression in both First and Second containers."
                );
            }

            this.firstContainer = firstContainer;
            this.secondContainer = secondContainer;
        }

        /// <summary>
        /// Gets first content container.
        /// </summary>
        public IExpression[] FirstContainer
        {
            get { return this.firstContainer; }
        }

        /// <summary>
        /// Gets second content container.
        /// </summary>
        public IExpression[] SecondContainer
        {
            get { return this.secondContainer; }
        }
    }
}
