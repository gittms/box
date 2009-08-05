using System;
using System.Data;
using System.Collections.Generic;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.ObjectSql.Test
{
    public class TestDatabase : ObjectSql.Database
    {
        public override void Init(string ConnectionString)
        {
            this.drawer = new Implementation.MsSql.Drawer();
        }

        protected override IDbConnection GetConnection()
        {
            throw new NotImplementedException();
        }

        protected override IDbCommand GetCommand()
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
    }

    public static class TestUtils
    {
        public static ObjectSql.Database Database
        {
            get
            {
                ObjectSql.Database db = new TestDatabase(); db.Init("");
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
