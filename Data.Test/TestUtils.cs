using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using Definitif.Data;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.Test
{
    public class TestDatabase : Data.Database
    {
        public override void Init(string ConnectionString)
        {
            this.drawer = new Implementation.MsSql.Drawer();
        }

        public override DbCommand GetCommand()
        {
            throw new NotImplementedException();
        }

        protected override void UpdateSchema()
        {
            throw new NotImplementedException();
        }

        protected override Drawer GetDrawer()
        {
            throw new NotImplementedException();
        }

        protected override DbConnection GetDatabaseConnection()
        {
            throw new NotImplementedException();
        }
    }

    public static class TestUtils
    {
        public static Data.Database Database
        {
            get
            {
                Data.Database db = new TestDatabase(); db.Init("");
                db.Add(new Table("Table"));
                db["Table"].Add(new Column("ID"));
                db["Table"].Add(new Column("Name"));
                db.Add(new Table("Chair"));
                db["Chair"].Add(new Column("ID"));
                db["Chair"].Add(new Column("TableID"));
                db["Chair"].Add(new Column("Name"));
                return db;
            }
        }
    }
}
