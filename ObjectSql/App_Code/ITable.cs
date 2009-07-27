using System;
using System.Collections;
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
}
