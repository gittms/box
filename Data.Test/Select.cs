using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Definitif.Data.Query;

namespace Definitif.Data.Test.Query
{
    [TestClass]
    public class SelectTest
    {
        public class SomeModelTable
        {
            public Column exp { get; set; }
        }

        public class SomeModel : IModel
        {
            public Id Id
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
            public int Version
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
            public IMapper IMapper()
            {
                throw new NotImplementedException();
            }

            public void SubscribeId(Id id)
            {
                throw new NotImplementedException();
            }

            public SomeModelTable C
            {
                get
                {
                    return new SomeModelTable()
                    {

                    };
                }
            }
        }

        [TestMethod, Priority(10)]
        [Description("Select query drawing.")]
        public void SelectDraw()
        {
            var query = new Select<SomeModel>().Where(m => 
                // exp: 0 .. 10; 100 .. +
                (m.C.exp > 0 & m.C.exp < 10) | m.C.exp > 100);
        }
    }
}
