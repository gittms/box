using System;
using System.Data;
using System.Data.SqlClient;
using Definitif.Data;

namespace Definitif.Data.ObjectSql.Implementation.MySql
{
    public sealed class Database : Data.Database
    {
        protected override ObjectSql.Drawer GetDrawer()
        {
            return new Drawer();
        }

        public override IDbConnection GetConnection()
        {
            throw new NotImplementedException();
        }

        public override IDbCommand GetCommand()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateSchema()
        {
            throw new NotImplementedException();
        }
    }
}
