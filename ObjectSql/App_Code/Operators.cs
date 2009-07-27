using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents generic operators template.
    /// </summary>
    public abstract class Operators
    {
        // Operator == is used to represent
        // constructions like:
        // 1. [Column] = [Column]
        // 2. [Column] = Value
        // 3. [Column] IN (Value, Value)
        //
        // Also used for expressions of
        // value changings, e.i.:
        // 1. SET [Column] = Value
        public static IExpression operator ==(Operators First, object Second)
        {
            return new Expression.Equals(
                Expression.Expression.CreateContainer(First),
                Expression.Expression.CreateContainer(Second));
        }
        public static IExpression operator ==(Operators First, object[] Second)
        {
            return new Expression.Equals(
                Expression.Expression.CreateContainer(First),
                Expression.Expression.CreateContainer(Second));
        }
        // Operator + is used to represent
        // expressions like:
        // 1. [Column] + Value
        // for value changing and comparison
        // exspressions, i.e.:
        // 1. [Column] > [Column] + 10
        // 2. [Column] = [Column] + 10
        public static IExpression operator +(Operators First, object Second)
        {
            return new Expression.Summ(
                Expression.Expression.CreateContainer(First, Second));
        }
        public static IExpression operator -(Operators First, object Second)
        {
            return new Expression.Subs(
                Expression.Expression.CreateContainer(First, Second));
        }
        // Operator != is used to represent
        // constructions like:
        // 1. [Column] <> [Column]
        // 2. [Column] <> Value
        // 3. [Column] NOT IN (Value, Value)
        public static IExpression operator !=(Operators First, object Second)
        {
            return new Expression.NotEquals(
                Expression.Expression.CreateContainer(First),
                Expression.Expression.CreateContainer(Second));
        }
        public static IExpression operator !=(Operators First, object[] Second)
        {
            return new Expression.NotEquals(
                Expression.Expression.CreateContainer(First),
                Expression.Expression.CreateContainer(Second));
        }
        // Operator > is used to represent
        // constructions like:
        // 1. [Column] > [Column]
        // 2. [Column] > Value
        public static IExpression operator >(Operators First, object Second)
        {
            return new Expression.More(
                Expression.Expression.CreateContainer(First),
                Expression.Expression.CreateContainer(Second));
        }
        public static IExpression operator >=(Operators First, object Second)
        {
            return new Expression.MoreOrEquals(
                Expression.Expression.CreateContainer(First),
                Expression.Expression.CreateContainer(Second));
        }
        // Operator > is used to represent
        // constructions like:
        // 1. [Column] > [Column]
        // 2. [Column] > Value
        public static IExpression operator <(Operators First, object Second)
        {
            return new Expression.Less(
                Expression.Expression.CreateContainer(First),
                Expression.Expression.CreateContainer(Second));
        }
        public static IExpression operator <=(Operators First, object Second)
        {
            return new Expression.LessOrEquals(
                Expression.Expression.CreateContainer(First),
                Expression.Expression.CreateContainer(Second));
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
