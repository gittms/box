using System;
using System.Collections;
using System.Collections.Generic;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents expression.
    /// </summary>
    public class Expression : IEnumerable
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

        internal List<Expression> GetMembers()
        {
            List<Expression> result = new List<Expression>();

            if (this.Type == ExpressionType.And ||
                this.Type == ExpressionType.Or)
            {
                foreach (Expression obj in this.Container)
                {
                    result.AddRange(obj.GetMembers());
                }
            }
            else
            {
                result.Add(this);

                // Second part of expression can also be an
                // expression, so iterating through container
                // and calling GetMembers().
                for (int i = 1; i < this.Container.Count; i++)
                {
                    if (this.Container[i] is Expression)
                    {
                        result.AddRange((this.Container[i] as Expression).GetMembers());
                    }
                }
            }

            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)(new ExpressionEnumerator(this));
        }
	}
}
