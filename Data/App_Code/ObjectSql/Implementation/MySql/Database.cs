using System;
using System.Data;
using System.Data.Common;
using Definitif.Data;

namespace Definitif.Data.ObjectSql.Implementation.MySql
{
    public sealed class Database : Data.Database
    {
        protected override ObjectSql.Drawer GetDrawer()
        {
            return new Drawer();
        }

        protected override DbConnection GetDatabaseConnection()
        {
            throw new NotImplementedException();
        }

        public override DbCommand GetCommand()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateSchema()
        {
            throw new NotImplementedException();
        }
    }
}
