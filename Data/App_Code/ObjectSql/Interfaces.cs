using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents ObjectSql table interface.
    /// </summary>
    public interface ITable
    {
        string Name { get; }

        Column this[string Column] { get; }
        Dictionary<string, Column> Columns { get; }
    }

    /// <summary>
    /// Represents ObjectSql column interface.
    /// </summary>
    public interface IColumn
    {
        string Name { get; }
        ITable Table { get; }
    }

    /// <summary>
    /// Represents query expression interface.
    /// </summary>
    public interface IExpression
    { }

    /// <summary>
    /// Represents ObjectSql query interface.
    /// </summary>
    public interface IQuery : ICloneable
    { }

    /// <summary>
    /// Represents ObjectSql joinable interface.
    /// </summary>
    public interface IJoinable : ITable
    { }
}
