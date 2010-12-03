using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Definitif.Data.Queries;
using Definitif.Data.Test.Models;

namespace Definitif.Data.Test
{
    [TestClass]
    public class DatabaseTest
    {
        [TestMethod, Priority(50)]
        [Description("Complex database test.")]
        public void Database()
        {
            Database db = global::Core.Database;
            // Removing all tables from database.
            try
            {
                db.DropTable(new Table("Tables"));
                db.DropTable(new Table("Chairs"));
                db.DropTable(new Table("Chair2Chair"));
            }
            // Don't mind if there's already no such
            // tables in database.
            catch (IndexOutOfRangeException) { }

            // Creating new tables for our needs.
            db.CreateTable("Tables",
                new Column("Name", "varchar(255)"));
            db.CreateTable("Chairs",
                new Column("TableId", "int"),
                new Column("Name", "varchar(255)"));
            db.CreateTable("Chair2Chair",
                new Column("FirstId", "int"),
                new Column("SecondId", "int"),
                new Column("Owner", "varchar(255)"));

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

            // Selecting ordered and limited list (1, 10, 2, [3, 4]...).
            chairs = new Select<Models.Chair>().OrderBy(m => m.C.Name.Asc).Limit(3, 2).Read();
            Assert.AreEqual(2, chairs.Length);
            Assert.AreEqual("Chair #4", chairs[1].Name);

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

            // Testing many to many relation.
            Models.ChairToChair link = new Models.ChairToChair()
            {
                First = chairs[0],
                Second = chairs[1],
                Owner = "ialubimii",
            };
            link.Save();

            Assert.AreEqual(1, chairs[0].GetChairs().Length);
            Assert.AreEqual(0, chairs[2].GetChairs().Length);
        }
    }
}
