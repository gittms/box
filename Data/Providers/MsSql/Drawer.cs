using System;

namespace Definitif.Data.Providers.MsSql
{
    public sealed class Drawer : Query.Drawer
    {
        public override string Identity
        {
            get { return "SELECT @@IDENTITY"; }
        }
    }
}
