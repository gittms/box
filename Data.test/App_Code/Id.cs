using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Definitif.Data.Queries;

namespace Definitif.Data.Test
{
    [TestClass]
    public class IdTest
    {
        private Query query;

        [TestMethod, Priority(50)]
        [Description("Generic Id object test.")]
        public void GenericIdTest()
        {
            Id empty = Id.Empty;
            Id one = new Id(1);
            Id two = null;

            Assert.AreEqual(true, empty == two);
            Assert.AreEqual(true, empty == null);
            Assert.AreEqual(false, one == null);
            Assert.AreEqual(true, one == new Id(1));
            Assert.AreEqual(true, two == null);
            Assert.AreEqual(false, new Id(500) == null);
            Assert.AreEqual(false, Id.Empty == one);
            Assert.AreEqual(false, new Id(5) == one);
        }
    }
}
