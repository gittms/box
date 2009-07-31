using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Definitif.Data.ObjectSql;

namespace Definitif.Data.ObjectSql.Test
{
    [TestClass]
    public class Database
    {
        private ObjectSql.Database database
            = new Implementation.MsSql.Database();

        [TestMethod, Priority(15)]
        [Description("Database Init() test.")]
        public void DatabaseInit()
        {
            this.database.Init(
                @"Data Source=.\SQLEXPRESS2005;Initial Catalog=ObjectSql;Persist Security Info=True;" +
                "User ID=sa;Password=tirvyind;Pooling=True;Min Pool Size=0;Max Pool Size=100");

            Assert.AreEqual(
                DatabaseState.Initialized,
                this.database.State,
                "Database state is not initialized.");

            Assert.IsTrue(
                this.database.Tables.ContainsKey("Tables") &&
                this.database.Tables.ContainsKey("Chairs"),
                "Tables schema loading failed.");

            Assert.IsTrue(
                this.database["Tables"].Columns.ContainsKey("ID") &&
                this.database["Tables"].Columns.ContainsKey("Name"),
                "Columns schema loading failed.");
        }

        [TestMethod, Priority(5)]
        [Description("Database Execute() test.")]
        public void DatabaseExecute()
        {
            if (this.database.State != DatabaseState.Initialized) this.DatabaseInit();

            Table table = this.database["Tables"];
            this.database.Execute(
                new Query.Insert(table["ID"] == 1, table["Name"] == "My first chair"));
        }
    }
}