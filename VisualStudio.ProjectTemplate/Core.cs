using System;
using System.Collections.Generic;
using Definitif;
using Definitif.Data;

/// <summary>
/// Represents Definitif Box Library core.
/// </summary>
[ConnectionStringKey("Core")]
public class Core
{
    /// <summary>
    /// Statically initializes core singleton.
    /// </summary>
    static Core()
    {
        core = new Definitif.Core(typeof(Core));
    }

    protected static Definitif.Core core;
    /// <summary>
    /// Gets core database object.
    /// </summary>
    public static Database Database
    {
        get
        {
            return core.Database;
        }
    }
}
