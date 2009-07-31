using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.ObjectSql.Test
{
    [TestClass]
    public class Insert
    {
        [TestMethod, Priority(10)]
        [Description("Query.Insert Draw() test.")]
        public void InsertDraw()
        {
            ObjectSql.Database db = TestUtils.Database;

            Assert.AreEqual(
                "INSERT INTO Table ( Table.[Name], Table.[ID] ) VALUES ( 'My Name', 1 )",
                db.Drawer.Draw(
                    new Query.Insert(
                        db["Table"]["Name"] == "My Name",
                        db["Table"]["ID"] == 1)
                        {
                            INTO = db["Table"]
                        }
                ),
                "Common insert draw failed.");

            Table table = db["Chair"];

            Assert.AreEqual(
                "INSERT INTO Chair ( Chair.[ID], Chair.[TableID], Chair.[Name] ) VALUES ( 123321, 3321, 'Billy''s favorite chair' )",
                db.Drawer.Draw(
                    new Query.Insert()
                    {
                        INTO = table,
                        VALUES =
                        {
                            table["ID"] == 123321,
                            table["TableID"] == 3321,
                            table["Name"] == "Billy's favorite chair"
                        }
                    }
                ),
                "Referenced insert draw failed.");

            Assert.AreEqual(
                "INSERT INTO Chair ( Chair.[ID], Chair.[TableID], Chair.[Name] ) VALUES ( 12, 1, '12th chair' )",
                db.Drawer.Draw(
                    new Query.Insert(table,
                        table["ID"] == 12,
                        table["TableID"] == 1,
                        table["Name"] == "12th chair")
                ),
                "Short form draw failed.");

            Assert.AreEqual(
                "INSERT INTO Chair ( Chair.[ID], Chair.[TableID], Chair.[Name] ) VALUES ( 11, 1, '42' )",
                db.Drawer.Draw(
                    new Query.Insert(
                        table["ID"] == 11,
                        table["TableID"] == 1,
                        table["Name"] == (42).ToString())
                ),
                "Insert with auto table reference failed.");
        }

        [TestMethod, Priority(1)]
        [Description("Query.Insert Draw() performance test (limit: 800ms for 100 000 iterations).")]
        public void InsertDrawPerformance()
        {
            ObjectSql.Database db = TestUtils.Database;
            DateTime start = DateTime.Now;
            TimeSpan time;

            for (int i = 0; i < 100000; i++)
            {
                string result = db.Drawer.Draw(
                    new Query.Insert(
                        db["Chair"]["Name"] == "My Name",
                        db["Chair"]["ID"] == 1,
                        db["Chair"]["TableID"] == 100)
                        {
                            INTO = db["Chair"]
                        }
                );
            }

            time = DateTime.Now - start;
            Assert.IsTrue(
                time <= new TimeSpan(0, 0, 0,0, 800),
                "100 000 common inserts rendering took " + time.ToString() + ".");
        }
    }
}
