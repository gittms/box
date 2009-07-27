using System;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents ObjectSql column interface.
    /// </summary>
    public interface IColumn
    {
        string Name { get; }

        ITable Table { get; }
    }
}
