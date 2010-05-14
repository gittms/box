using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Definitif.Data.Queries
{
    public abstract class Operators
    {
        public static Expression operator ==(Operators column, object value)
        {
            return new Expression()
            {
                Type = ExpressionType.Equals,
                Container = { column, value },
            };
        }
        public static Expression operator ==(Operators column, object[] value)
        {
            return new Expression()
            {
                Type = ExpressionType.Equals,
                Container = { column, value },
            };
        }

        public static Expression operator !=(Operators column, object value)
        {
            return new Expression()
            {
                Type = ExpressionType.NotEquals,
                Container = { column, value },
            };
        }
        public static Expression operator !=(Operators column, object[] value)
        {
            return new Expression()
            {
                Type = ExpressionType.NotEquals,
                Container = { column, value },
            };
        }

        public static Expression operator >(Operators column, object value)
        {
            return new Expression()
            {
                Type = ExpressionType.Greater,
                Container = { column, value },
            };
        }
        public static Expression operator >=(Operators column, object value)
        {
            return new Expression()
            {
                Type = ExpressionType.GreaterOrEquals,
                Container = { column, value },
            };
        }

        public static Expression operator <(Operators column, object value)
        {
            return new Expression()
            {
                Type = ExpressionType.Less,
                Container = { column, value },
            };
        }
        public static Expression operator <=(Operators column, object value)
        {
            return new Expression()
            {
                Type = ExpressionType.LessOrEquals,
                Container = { column, value },
            };
        }
    }
}
