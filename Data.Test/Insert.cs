using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Definitif.Data;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.Test
{
    [TestClass]
    public class Insert
    {
        [TestMethod, Priority(10)]
        [Description("Query.Insert Draw() test.")]
        public void InsertDraw()
        {
            Data.Database db = TestUtils.Database;

            Assert.AreEqual(
                "INSERT INTO Table ( Table.[Name], Table.[ID] ) VALUES ( 'My Name', 1 )",
                db.Drawer.Draw(
                    new ObjectSql.Query.Insert(
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
                    new ObjectSql.Query.Insert()
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
                    new ObjectSql.Query.Insert(table,
                        table["ID"] == 12,
                        table["TableID"] == 1,
                        table["Name"] == "12th chair")
                ),
                "Short form draw failed.");

            Assert.AreEqual(
                "INSERT INTO Chair ( Chair.[ID], Chair.[TableID], Chair.[Name] ) VALUES ( 11, 1, '42' )",
                db.Drawer.Draw(
                    new ObjectSql.Query.Insert(
                        table["ID"] == 11,
                        table["TableID"] == 1,
                        table["Name"] == (42).ToString())
                ),
                "Insert with auto table reference failed.");

            Assert.AreEqual(
                "INSERT INTO Chair ( Chair.[ID], Chair.[TableID], Chair.[Name] ) VALUES ( 13, 3, '2009-01-04 12:30:00' )",
                db.Drawer.Draw(
                    new ObjectSql.Query.Insert(
                        table["ID"] == 13,
                        table["TableID"] == 3,
                        table["Name"] == new DateTime(2009, 01,04, 12, 30, 0))
                ),
                "Insert with DateTime conversion failed.");
        }

        [TestMethod, Priority(1)]
        [Description("Query.Insert Draw() performance test (limit: 650ms for 100 000 iterations).")]
        public void InsertDrawPerformance()
        {
            Data.Database db = TestUtils.Database;
            DateTime start = DateTime.Now;
            TimeSpan time;

            for (int i = 0; i < 100000; i++)
            {
                string result = db.Drawer.Draw(
                    new ObjectSql.Query.Insert(
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
                time <= new TimeSpan(0, 0, 0,0, 650),
                "100 000 common inserts rendering took " + time.ToString() + ".");
        }
    }
}
