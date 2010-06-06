﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Definitif.Data.Queries;

namespace Definitif.Data.Test
{
    [TestClass]
    public class DatabaseTest
    {
        [TestMethod, Priority(50)]
        [Description("Complex database test.")]
        public void Database()
        {
            // Cleaning up result.
            new Delete<Models.Table>().Execute();
            new Delete<Models.Chair>().Execute();

            // Creating table instance.
            Models.Table table = new Models.Table()
            {
                Name = "Table name",
            };
            table.Save();
            Assert.AreNotEqual(Id.Empty, table.Id);

            // Creating multiple chairs.
            for (int i = 1; i <= 10; i++)
            {
                Models.Chair chair = new Models.Chair()
                {
                    Name = "Chair #" + i.ToString(),
                    Table = table,
                };
                chair.Save();
                Assert.AreNotEqual(Id.Empty, chair.Id);
            }

            // Selecting multiple chairs.
            Models.Chair[] chairs = new Select<Models.Chair>().Where(m => m.C.Table.Id == table.Id).Read();
            Assert.AreEqual(10, chairs.Length);

            // Selecting ordered and limited list (1, 10, 2, 3, ...).
            chairs = new Select<Models.Chair>().OrderBy(m => m.C.Name.Asc).Limit(3, 2).Read();
            Assert.AreEqual(2, chairs.Length);
            Assert.AreEqual("Chair #3", chairs[1].Name);

            Models.Chair someChair = Models.Chair.Get(chairs[1].Id);
            Assert.AreEqual(chairs[1].Id, someChair.Id);

            // Updating some data.
            new Update<Models.Chair>().Set(m => m.C.Name == m.C.Name + " Updated").Where(m => m.C.Table == table).Execute();
            chairs = new Select<Models.Chair>().Where(m => m.C.Name.EndsWith("Updated")).Read();
            Assert.AreEqual(10, chairs.Length);

            // Removing some data.
            new Delete<Models.Chair>().Top(5).Execute();
            chairs = new Select<Models.Chair>().Read();
            Assert.AreEqual(5, chairs.Length);
        }
    }
}