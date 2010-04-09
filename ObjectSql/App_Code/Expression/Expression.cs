using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Expression
{
    /// <summary>
    /// Represents expression abstract object.
    /// </summary>
    public abstract class Expression : Operators, IExpression
    {
        /// <summary>
        /// Creates IExpression container from parametrized objects array.
        /// </summary>
        /// <param name="Objects">Parametrized objects array.</param>
        /// <returns>IExpression container.</returns>
        public static IExpression[] CreateContainer(params object[] Objects)
        {
            IExpression[] list = new IExpression[Objects.Length];
            for (int i = 0; i < list.Length; i++)
            {
                if (Objects[i] == null)
                {
                    return new IExpression[] { null };
                }
                else if (!(Objects[i] is IExpression))
                {
                    list[i] = new Object(Objects[i]);
                }
                else
                {
                    list[i] = Objects[i] as IExpression;
                }
            }
            return list;
        }
    }
}
