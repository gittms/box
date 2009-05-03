using System;
using System.Data;

namespace Definitif.Data.ObjectSql.Implementation.MsSql
{
    public class Database : ObjectSql.Database
    {
        public override void Init(string ConnectionString)
        {
            throw new NotImplementedException();
        }

        public override ObjectSql.Drawer Drawer
        {
            get { return new MsSql.Drawer(); }
        }
    }
}
