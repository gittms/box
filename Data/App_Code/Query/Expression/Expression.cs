using System;
using System.Collections;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents expression.
    /// </summary>
    public partial class Expression
    {
        /// <summary>
        /// Gets or sets expression type.
        /// </summary>
        public ExpressionType Type { get; set; }
        /// <summary>
        /// Gets or sets expression container.
        /// </summary>
        public ArrayList Container { get; set; }

        public Expression()
        {
            this.Container = new ArrayList();
        }
		
		public static Expression operator &(Expression left, Expression right)
		{
			if (left.Type == ExpressionType.And)
			{
				left.Container.Add(right);
				return left;
			}
			else if (right.Type == ExpressionType.And)
			{
				right.Container.Add(left);
				return right;
			}
			else
			{
				return new Expression()
				{
					Type = ExpressionType.And,
					Container = { left, right },
				};
			}
		}
		public static Expression operator |(Expression left, Expression right)
		{
			if (left.Type == ExpressionType.Or)
			{
				left.Container.Add(right);
				return left;
			}
			else if (right.Type == ExpressionType.Or)
			{
				right.Container.Add(left);
				return right;
			}
			else
			{
				return new Expression()
				{
					Type = ExpressionType.Or,
					Container = { left, right },
				};
			}
		}
	}
}
