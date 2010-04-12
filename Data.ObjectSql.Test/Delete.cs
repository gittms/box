using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.ObjectSql.Test
{
    [TestClass]
    public class Delete
    {
        [TestMethod, Priority(10)]
        [Description("Query.Delete Draw() test.")]
        public void DeleteDraw()
        {
            ObjectSql.Database db = TestUtils.Database;

            Assert.AreEqual(
                "DELETE FROM Table WHERE Table.[ID] > 3",
                db.Drawer.Draw(
                    new Query.Delete(
                        db["Table"], db["Table"]["ID"] > 3)
                ),
                "Common delete draw failed.");

            Assert.AreEqual(
                "DELETE FROM Table WHERE Table.[ID] NOT IN ( SELECT Chair.[TableID] FROM Chair )",
                db.Drawer.Draw(
                    new Query.Delete(
                        db["Table"])
                        {
                            WHERE =
                            {
                                db["Table"]["ID"] != new Query.Select(db["Chair"]["TableID"])
                            }
                        }
                ),
                "Delete with subselect draw failed.");

            Assert.AreEqual(
                "DELETE FROM Table INNER JOIN Chair ON Table.[ID] = Chair.[TableID] WHERE Chair.[Name] = 'One'",
                db.Drawer.Draw(
                    new Query.Delete(
                        db["Table"].INNERJOIN(db["Chair"], db["Table"]["ID"] == db["Chair"]["TableID"]))
                        {
                            WHERE =
                            {
                                db["Chair"]["Name"] == "One"
                            }
                        }
                ),
                "Delete with join draw failed.");

            Assert.AreEqual(
                "DELETE TOP 10 FROM Chair",
                db.Drawer.Draw(
                    new Query.Delete()
                        .From(db["Chair"])
                        .Top(10)
                ),
                "Delete query with linq-chain-based query failed.");

            Assert.AreEqual(
                "WITH _RowCounter AS ( SELECT , ROW_NUMBER() OVER( ORDER BY ( SELECT 0 ) ) AS [_RowNum] FROM Chair WHERE Chair.[TableID] > 4 ) " +
                "DELETE FROM _RowCounter WHERE [_RowNum] >= 10 AND [_RowNum] < 20",
                db.Drawer.Draw(
                    new Query.Delete(db["Chair"])
                    {
                        WHERE = { db["Chair"]["TableID"] > 4 },
                        LIMIT = new Limit(10, 10)
                    }
                ),
                "Paged delete query failed.");
        }

        [TestMethod, Priority(1)]
        [Description("Query.Delete Draw() performance test (limit: 500ms for 100 000 iterations).")]
        public void DeleteDrawPerformance()
        {
            ObjectSql.Database db = TestUtils.Database;
            DateTime start = DateTime.Now;
            TimeSpan time;

            for (int i = 0; i < 100000; i++)
            {
                string result = db.Drawer.Draw(
                    new Query.Delete(
                        db["Table"], db["Table"]["ID"] > 3, db["Table"]["Name"].LENGTH < 100)
                );
            }

            time = DateTime.Now - start;
            Assert.IsTrue(
                time <= new TimeSpan(0, 0, 0, 0, 500),
                "100 000 common inserts rendering took " + time.ToString() + ".");
        }
    }
}
