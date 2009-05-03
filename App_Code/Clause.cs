using System;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents query clause.
    /// </summary>
    public class Clause : IExpression
    {
        /// <summary>
        /// Creates new AND clause group.
        /// </summary>
        /// <param name="Clauses">Clauses to include to clause group.</param>
        /// <returns>New AND clause group.</returns>
        public static IExpression AND(params IExpression[] Clauses)
        {
            return new
                Expression.AND(Clauses);
        }
        /// <summary>
        /// Creates new OR clause group.
        /// </summary>
        /// <param name="Clauses">Clauses to include to clause group.</param>
        /// <returns>New OR clause group.</returns>
        public static IExpression OR(params IExpression[] Clauses)
        {
            return new
                Expression.OR(Clauses);
        }

        // AUTODOC: Clause.LIKE(string Expression, IColumn Column)
        public static IExpression LIKE(string Expression, IColumn Column)
        {
            return new
                Expression.LIKE(Expression, Column);
        }

        // AUTODOC: Clause.CONTAINS(string Expression, params IColumn[] Columns)
        public static IExpression CONTAINS(string Expression, params IColumn[] Columns)
        {
            return new
                Expression.CONTAINS(Expression, Columns);
        }
    }
}
