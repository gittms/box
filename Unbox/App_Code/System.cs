using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NDesk.Options;

namespace Definitif.Box.Unbox
{
    /// <summary>
    /// Represents static class for system information retrieval.
    /// </summary>
    public static class System
    {
        /// <summary>
        /// Checking if running under Mono runtime.
        /// </summary>
        public static bool IsMonoRuntime
        {
            get
            {
                Type t = Type.GetType("Mono.Runtime");
                return (t != null);
            }
        }

        /// <summary>
        /// Checking if running on Windows platform.
        /// </summary>
        public static bool IsWindowsOs
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return ((p == 4) || (p == 6) || (p == 128));
            }
        }
    }
}
