using System;
using System.Collections;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents expression enumerator.
    /// </summary>
    public class ExpressionEnumerator : IEnumerator
    {
        private Expression[] list;

        public ExpressionEnumerator(Expression expression)
        {
            list = expression.GetMembers().ToArray();
        }

        int position = -1;

        public bool MoveNext()
        {
            position++;
            return (position < list.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public Expression Current
        {
            get
            {
                try
                {
                    return list[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
	}
}
