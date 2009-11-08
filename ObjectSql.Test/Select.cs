using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.ObjectSql.Test
{
    [TestClass]
    public class Select
    {
        [TestMethod, Priority(10)]
        [Description("Query.Select Draw() test.")]
        public void SelectDraw()
        {
            ObjectSql.Database db = TestUtils.Database;

            Assert.AreEqual(
                "SELECT Table.[Name] FROM Table",
                db.Drawer.Draw(
                    new Query.Select(
                        db["Table"]["Name"])
                        {
                            FROM = { db["Table"] }
                        }
                ),
                "Single select draw failed.");

            Assert.AreEqual(
                "SELECT Table.[ID] AS [GUID] FROM Table",
                db.Drawer.Draw(
                    new Query.Select(
                        db["Table"]["ID"] == new ColumnAlias("GUID"))
                ),
                "Single select with column alias and UpdateFrom() draw failed.");

            Assert.AreEqual(
                "SELECT Table.[ID], Table.[Name] FROM Table",
                db.Drawer.Draw(
                    new Query.Select(
                        db["Table"]["ID"], db["Table"]["Name"])
                ),
                "Multiple select with UpdateFrom() draw failed.");

            Assert.AreEqual(
                "SELECT Table.[Name], Chair.[Name] FROM Chair INNER JOIN Table ON Chair.[TableID] = Table.[ID] ORDER BY Table.[Name]",
                db.Drawer.Draw(
                    new Query.Select(
                        db["Table"]["Name"], db["Chair"]["Name"])
                        {
                            FROM = { db["Chair"].INNERJOIN(db["Table"], db["Chair"]["TableID"] == db["Table"]["ID"]) },
                            ORDERBY = { db["Table"]["Name"] }
                        }
                ),
                "Multiple joined select with sorting draw failed.");

            Assert.AreEqual(
                "SELECT Table.[Name], Chair.[Name] FROM Chair INNER JOIN Table ON Chair.[TableID] = Table.[ID] INNER JOIN Chair ON Chair.[TableID] > 2",
                db.Drawer.Draw(
                    new Query.Select(
                        db["Table"]["Name"], db["Chair"]["Name"])
                        {
                            FROM = { db["Chair"].INNERJOIN(db["Table"], db["Chair"]["TableID"] == db["Table"]["ID"])
                                                .INNERJOIN(db["Chair"], db["Chair"]["TableID"] > 2) }
                        }
                ),
                "Multiple aranged joins select draw failed.");

            Assert.AreEqual(
                "SELECT MAX( Table.[ID] ) FROM Table WHERE Table.[Name] IN ( 'Long', 'Cycle' )",
                db.Drawer.Draw(
                    new Query.Select(
                        Column.MAX(db["Table"]["ID"]))
                        {
                            FROM = { db["Table"] },
                            WHERE = { db["Table"]["Name"] == new string[] { "Long", "Cycle" } }
                        }
                ),
                "Aggregated select with object collection draw failed.");

            Assert.AreEqual(
                "SELECT Table.[ID] FROM Table, Chair WHERE ( Table.[Name] IS NOT NULL AND Table.[ID] >= Chair.[ID] + 2 )",
                db.Drawer.Draw(
                    new Query.Select(
                        db["Table"]["ID"])
                        {
                            FROM = { db["Table"], db["Chair"] },
                            WHERE = { db["Table"]["Name"] != db.NULL, db["Table"]["ID"] >= db["Chair"]["ID"] + 2 }
                        }
                ),
                "Greater than select with numeric operation draw failed.");

            Assert.AreEqual(
                "SELECT Table.[ID] AS [Table.ID], Table.[Name] AS [Table.Name] FROM Table WHERE Table.[Name] IS NOT NULL",
                db.Drawer.Draw(
                    new Query.Select(
                        db["Table"]["**"])
                        {
                            WHERE = { db["Table"]["Name"] != db.NULL }
                        }
                ),
                "Select using Table.** draw failed."
                );

            Assert.AreEqual(
                "SELECT Table.* FROM Table",
                db.Drawer.Draw(
                    new Query.Select(db["Table"]["*"])
                ),
                "All from table select draw failed.");

            Assert.AreEqual(
                "SELECT DISTINCT( Table.[ID] ) FROM Table",
                db.Drawer.Draw(
                    new Query.Select(Column.DISTINCT(db["Table"]["ID"]))
                ),
                "Distinct ID select draw failed.");

            Assert.AreEqual(
                "SELECT Table.* FROM Table WHERE Table.[ID] > 10 GROUP BY Table.[ID]",
                db.Drawer.Draw(
                    new Query.Select()
                        .Values(db["Table"]["*"])
                        .From(db["Table"])
                        .Where(db["Table"]["ID"] > 10)
                        .GroupBy(db["Table"]["ID"])
                ),
                "Linq-style select draw failed.");
        }

        [TestMethod, Priority(1)]
        [Description("Query.Select Draw() performance test (limit: 750ms for 100 000 iterations).")]
        public void SelectDrawPerformance()
        {
            ObjectSql.Database db = TestUtils.Database;
            DateTime start = DateTime.Now;
            TimeSpan time;

            for (int i = 0; i < 100000; i++)
            {
                string result = db.Drawer.Draw(
                    new Query.Select(
                        db["Table"]["Name"], db["Chair"]["Name"], db["Table"]["ID"])
                        {
                            FROM =
                            {
                                db["Chair"].INNERJOIN(
                                    db["Table"], 
                                    db["Chair"]["TableID"] == db["Table"]["ID"]) 
                            },
                            ORDERBY = 
                            {
                                db["Table"]["ID"]
                            }
                        }
                );
            }

            time = DateTime.Now - start;
            Assert.IsTrue(
                time <= new TimeSpan(0, 0, 0, 0, 750),
                "100 000 joined selects rendering took " + time.ToString() + ".");
        }
    }
}
