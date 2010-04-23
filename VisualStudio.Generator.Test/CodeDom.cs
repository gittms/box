using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Definitif.VisualStudio.Generator;

namespace Definitif.VisualStudio.Generator.Test
{
    [TestClass]
    [DeploymentItem(@"Tests\CodeDomCommon.box", "Tests")]
    public class CodeDomTest
    {
        [TestMethod, Priority(15)]
        [Description("CodeDom common test.")]
        public void CodeDomCommon()
        {
            CodeDom codeDom = CodeDom.ParseFile(@"Tests\CodeDomCommon.box");

            // Checking namespaces.
            Assert.AreEqual(2, codeDom.Namespaces.Count);
            Assert.AreEqual(typeof(DefaultNamespace), codeDom.Namespaces[1].ParentNamespace.GetType());
            Assert.AreEqual("TestNamespace", codeDom.Namespaces[1].Name);

            // Checking models counters.
            Assert.AreEqual(1, codeDom.Namespaces[0].Models.Count);
            Assert.AreEqual(3, codeDom.Namespaces[1].Models.Count);

            // Checking members counters.
            Assert.AreEqual(3, codeDom.Namespaces[0].Models[0].Members.Count);
            Assert.AreEqual(3, codeDom.Namespaces[1].Models[0].Members.Count);
            Assert.AreEqual(2, codeDom.Namespaces[1].Models[1].Members.Count);
            Assert.AreEqual(1, codeDom.Namespaces[1].Models[2].Members.Count);

            // Checking modifiers.
            Assert.AreEqual((int)Modifier.Foreign_key, (int)codeDom.Namespaces[0].Models[0].Members[0].Modifiers);
            Assert.AreEqual((int)(Modifier.Public | Modifier.Static), (int)codeDom.Namespaces[1].Models[1].Members[1].Modifiers);
        }

        [TestMethod, Priority(25)]
        [Description("CodeDom namespaces parsing exception test.")]
        [ExpectedException(typeof(FormatException), AllowDerivedTypes = true)]
        public void CodeDomNamespacesException()
        {
            // Namespace declared inside model.
            CodeDom codeDom = CodeDom.Parse(
            @"namespace TestNamespace {
                namespace InnerNamespace {
                    " + "[\"Models\"]" + @"
                    model Model {
                        namespace InnerNamespace {
                        }
                    }
                }
            }");
        }
    }
}
