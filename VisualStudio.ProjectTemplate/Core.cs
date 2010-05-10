using System;
using System.Collections.Generic;
using Definitif;
using Definitif.Data;

/// <summary>
/// Represents Definitif Box Library core.
/// </summary>
[ConnectionStringKey("Core")]
public class Core : Definitif.Core
{
    /// <summary>
    /// Statically initializes core singleton.
    /// </summary>
    static Core()
    {
        core = new Definitif.Core(typeof(Core));
    }
}
