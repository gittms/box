using System;

namespace Definitif.Data.ObjectSql.Implementation.MsSql
{
    public class Drawer : ObjectSql.Drawer
    {
        public override string IDENTITY
        {
            get { return "SELECT @@IDENTITY"; }
        }
    }
}
