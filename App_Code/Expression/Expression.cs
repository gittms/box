using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql.Expression
{
    // AUTODOC: class Expression
    public abstract class Expression : Operators, IExpression
    {
        // AUTODOC: Expression.CreateContainer(params object[] Objects)
        public static IExpression[] CreateContainer(params object[] Objects)
        {
            IExpression[] list = new IExpression[Objects.Length];
            for (int i = 0; i < list.Length; i++)
            {
                if (Objects[i] == null)
                {
                    return
                        new IExpression[] { null };
                }
                else if (!(Objects[i] is IExpression))
                {
                    list[i] = new Object(Objects[i]);
                }
                else
                {
                    list[i] = (IExpression)Objects[i];
                }
            }
            return list;
        }
    }
}
