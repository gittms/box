using System;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.Implementation.MySql
{
    public sealed class Drawer : ObjectSql.Drawer
    {
        public override string IDENTITY
        {
            get { return "SELECT LAST_INSERT_ID()"; }
        }
    }
}
