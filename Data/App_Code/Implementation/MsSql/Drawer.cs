using System;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.Implementation.MsSql
{
    public sealed class Drawer : ObjectSql.Drawer
    {
        public override string IDENTITY
        {
            get { return "SELECT @@IDENTITY"; }
        }
    }
}
