using System;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    // AUTODOC: class Drawer()
    public abstract partial class Drawer
    {
        // AUTODOC: Drawer.Except(object Object)
        private string Except(object Object)
        {
            #if DEBUG
            return " [UNKNOWN] ";
            #endif
            throw new NotImplementedException(
                String.Format(
                    "Handling of '{0}' is not implemented in Drawer.Draw().",
                    Object.GetType().ToString()
                ));
        }
    }
}
