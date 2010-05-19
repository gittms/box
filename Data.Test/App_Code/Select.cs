using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Definitif.Data.Queries;

namespace Definitif.Data.Test
{
    [TestClass]
    public class SelectTest
    {
        private Query query;

        [TestMethod, Priority(10)]
        [Description("Select query drawing.")]
        public void SelectDraw()
        {
            // Plain selects.
            query = new Select<Models.Table>().Where(m => m.C.Id == 1);
            Assert.AreEqual("SELECT Tables.* FROM Tables WHERE Tables.[Id] = 1", query.ToString());

            query = new Select<Models.Chair>().Where(m => m.C.Table.Id == 1);
            Assert.AreEqual("SELECT Chairs.* FROM Chairs WHERE Chairs.[TableId] = 1", query.ToString());

            // Value functions.
            query = new Select<Models.Table>().Where(m => m.C.Name.Length >= 10);
            Assert.AreEqual("SELECT Tables.* FROM Tables WHERE DATALENGTH(Tables.[Name]) >= 10", query.ToString());

            // Limiting functions.
            query = new Select<Models.Table>().Where(m => m.C.Id > 10).Top(100);
            Assert.AreEqual("SELECT TOP 100 FROM Tables WHERE Tables.[Id] > 10", query.ToString());

            query = new Select<Models.Table>().Where(m => m.C.Id > 10).Limit(100, 100);
            Assert.AreEqual("WITH _RowCounter AS ( SELECT Tables.*, ROW_NUMBER() OVER( ORDER BY ( SELECT 0 ) ) AS [_RowNum] FROM Tables WHERE Tables.[Id] > 10 ) " +
                "SELECT * FROM _RowCounter WHERE [_RowNum] >= 100 AND [_RowNum] < 200", query.ToString());

            query = new Select<Models.Table>().OrderBy(m => m.C.Name.Asc).Limit(100, 200);
            Assert.AreEqual("WITH _RowCounter AS ( SELECT Tables.*, ROW_NUMBER() OVER( ORDER BY Tables.[Name] ASC ) AS [_RowNum] FROM Tables ) " +
                "SELECT * FROM _RowCounter WHERE [_RowNum] >= 100 AND [_RowNum] < 300", query.ToString());

            // Logic functions.
            query = new Select<Models.Table>().Where(m =>
                (m.C.Name.Contains("Table") | m.C.Name.EndsWith("Name")) & m.C.Version > 1);
            Assert.AreEqual("SELECT Tables.* FROM Tables WHERE ( Tables.[Name] LIKE '%Table%' OR Tables.[Name] LIKE '%Name' ) AND Tables.[Version] > 1", query.ToString());

            // Ordering and grouping.
            query = new Select<Models.Table>().OrderBy(m => m.C.Name.Asc);
            Assert.AreEqual("SELECT * FROM Tables ORDER BY Tables.[Name] ASC", query.ToString());

            query = new Select<Models.Chair>().Fields(m => m.C.Name & m.C.Id.Count.As("Num")).GroupBy(m => m.C.Name);
            Assert.AreEqual("SELECT Chairs.[Name], COUNT(Chairs.[Id]) AS [Num] FROM Chairs GROUP BY Chairs.[Name]", query.ToString());

            // Joins detection.
            query = new Select<Models.Chair>().Where(m => m.C.Table.Name.StartsWith("Table"));
            Assert.AreEqual("SELECT Chairs.* FROM Chairs INNER JOIN Tables ON Chairs.[TableId] = Tables.[Id] WHERE Tables.[Name] LIKE 'Table%'", query.ToString());

            // Manual joins.
            query = new Select<Models.Chair>().InnerJoin<Models.Table>((m, j) => m.C.Id == j.C.Id).Where((m, j) => j.C.Name.Contains("Table"));
            Assert.AreEqual("SELECT Chairs.* FROM Chairs INNER JOIN Tables ON Chairs.[Id] = Tables.[Id] WHERE Tables.[Name] LIKE '%Table%'", query.ToString());
        }

        [TestMethod, Priority(5)]
        [Description("Insert query drawing.")]
        public void InsertDraw()
        {
            query = new Insert<Models.Table>().Values(m => m.C.Name == "Big Table");
            Assert.AreEqual("INSERT INTO Tables ([Name]) VALUES ('Big Table')", query.ToString());

            query = new Insert<Models.Chair>().Values(m => m.C.Name == "Small Chair" & m.C.Table.Id == 100);
            Assert.AreEqual("INSERT INTO Chairs ([Name], [TableId]) VALUES ('Small Chair', 100)", query.ToString());
        }

        [TestMethod, Priority(10)]
        [Description("Update query drawing.")]
        public void UpdateDraw()
        {
            query = new Update<Models.Chair>().Values(m => m.C.Name == "Small Chair").Where(m => m.C.Id < 10);
            Assert.AreEqual("UPDATE Chairs SET Chairs.[Name] = 'Small Chair' WHERE Chairs.[Id] < 10", query.ToString());
        }

        [TestMethod, Priority(5)]
        [Description("Delete query drawing.")]
        public void DeleteDraw()
        {
            query = new Update<Models.Table>().Where(m => m.C.Id > 100);
            Assert.AreEqual("DELETE FROM Tables WHERE Tables.[Id] > 100", query.ToString());
        }
    }
}
