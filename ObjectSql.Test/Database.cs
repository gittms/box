﻿using System;
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

            this.database.Execute(
                new Query.Delete(table));
        }

        [TestMethod, Priority(15)]
        [Description("Database ExecuteReader() test.")]
        public void DatabaseExecuteReader()
        {
            if (this.database.State != DatabaseState.Initialized) this.DatabaseInit();

            Table tables = this.database["Tables"],
                chairs = this.database["Chairs"];

            this.database.Execute(
                // Creating tables.
                new Query.Insert(tables["ID"] == 1, tables["Name"] == "My first table"),
                new Query.Insert(tables["ID"] == 2, tables["Name"] == "My second table"),
                new Query.Insert(tables["ID"] == 3, tables["Name"] == "My third table"),
                // Creating chairs
                new Query.Insert(chairs["ID"] == 1, chairs["TableID"] == 1, chairs["Name"] == "Chair for first table"),
                new Query.Insert(chairs["ID"] == 2, chairs["TableID"] == 1, chairs["Name"] == "Chair for first table"),
                new Query.Insert(chairs["ID"] == 3, chairs["TableID"] == 2, chairs["Name"] == "Chair for second table"),
                new Query.Insert(chairs["ID"] == 4, chairs["TableID"] == 3, chairs["Name"] == "Chair for third table"));

            Reader reader = this.database.ExecuteReader(
                new Query.Select(tables["ID"], tables["Name"]) { WHERE = { tables["ID"] > 0 } });

            while (reader.Read())
            {
                Assert.IsTrue(
                    ((int)reader["ID"] == 1 && (string)reader["Name"] == "My first table") ||
                    ((int)reader["ID"] == 2 && (string)reader["Name"] == "My second table") ||
                    ((int)reader["ID"] == 3 && (string)reader["Name"] == "My third table"),
                    "Common select reader failed.");
            }
            reader.Close();

            this.database.Execute(
                new Query.Delete(tables), new Query.Delete(chairs));
        }
    }
}
