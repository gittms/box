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
        [TestMethod, Priority(10)]
        [Description("Select query drawing.")]
        public void SelectDraw()
        {
            //var select = new Select<User>().Where(m =>
            //    m.C.Id < 100 | m.C.Id > 200);
        }
    }
}
