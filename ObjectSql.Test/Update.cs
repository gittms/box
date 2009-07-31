using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.ObjectSql.Test
{
    [TestClass]
    public class Update
    {
        [TestMethod, Priority(10)]
        [Description("Query.Update Draw() test.")]
        public void UpdateDraw()
        {
            ObjectSql.Database db = TestUtils.Database;

            Assert.AreEqual(
                "UPDATE Table SET Table.[Name] = Table.[Name] + ' updated' WHERE Table.[ID] < 100",
                db.Drawer.Draw(
                    new Query.Update(
                        db["Table"]["Name"] == db["Table"]["Name"] + " updated")
                    {
                        TABLES = { db["Table"] },
                        WHERE = 
                        {
                            db["Table"]["ID"] < 100
                        }
                    }
                ),
                "Common update with column reference draw failed.");

            Assert.AreEqual(
                "UPDATE Table SET Table.[ID] = Table.[ID] + 1",
                db.Drawer.Draw(
                    new Query.Update(
                        db["Table"]["ID"] == db["Table"]["ID"] + 1)
                ),
                "Update with auto table reference draw failed.");

            Assert.AreEqual(
                "UPDATE Table INNER JOIN Chair ON Table.[ID] = Chair.[TableID] SET Table.[Name] = Table.[Name] + ' (w. chairs)' WHERE Chair.[ID] > 4000",
                db.Drawer.Draw(
                    new Query.Update(db["Table"].INNERJOIN(
                        db["Chair"], db["Table"]["ID"] == db["Chair"]["TableID"]),
                        db["Table"]["Name"] == db["Table"]["Name"] + " (w. chairs)")
                        {
                            WHERE =
                            {
                                db["Chair"]["ID"] > 100 * 40
                            }
                        }
                ),
                "Update with joined tables draw failed.");

            Assert.AreEqual(
                "UPDATE Table SET Table.[ID] = Table.[ID] * 2 WHERE Table.[ID] NOT IN ( SELECT Chair.[TableID] FROM Chair )",
                db.Drawer.Draw(
                    new Query.Update(db["Table"],
                        db["Table"]["ID"] == db["Table"]["ID"] * 2)
                        {
                            WHERE =
                            {
                                db["Table"]["ID"] != new Query.Select(db["Chair"]["TableID"])
                            }
                        }
                ),
                "Update with embedded select draw failed.");
        }

        [TestMethod, Priority(1)]
        [Description("Query.Update Draw() performance test (limit: 1s 500ms for 100 000 iterations).")]
        public void UpdateDrawPerformance()
        {
            ObjectSql.Database db = TestUtils.Database;
            DateTime start = DateTime.Now;
            TimeSpan time;

            for (int i = 0; i < 100000; i++)
            {
                string result = db.Drawer.Draw(
                    new Query.Update(db["Table"].INNERJOIN(
                        db["Chair"], db["Table"]["ID"] == db["Chair"]["TableID"]),
                        db["Table"]["Name"] == db["Table"]["Name"] + " (w. chairs)")
                        {
                            WHERE =
                            {
                                db["Chair"]["ID"] > 100
                            }
                        }
                );
            }

            time = DateTime.Now - start;
            Assert.IsTrue(
                time <= new TimeSpan(0, 0, 0, 1, 500),
                "100 000 updates rendering took " + time.ToString() + ".");
        }
    }
}
