﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Definitif.VisualStudio.Generator;

namespace Definitif.VisualStudio.Generator.Test
{
    [TestClass]
    [DeploymentItem(@"Tests\CodeDomCommon.box", "Tests")]
    // Code generation test items.
    [DeploymentItem(@"Tests\Generator\Autodoc.box", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\Autodoc.result", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\Body.box", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\Body.result", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\ForeignKey.box", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\ForeignKey.result", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\PrimaryKey.box", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\PrimaryKey.result", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\ManyToMany.box", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\ManyToMany.result", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\Model.box", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\Model.result", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\Inheritance.box", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\Inheritance.result", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\MultipleInheritance.box", @"Tests\Generator")]
    [DeploymentItem(@"Tests\Generator\MultipleInheritance.result", @"Tests\Generator")]
    public class CodeDomTest
    {
        [TestMethod, Priority(15)]
        [Description("CodeDom common test.")]
        public void CodeDomCommon()
        {
            CodeDom codeDom = CodeDom.ParseFile(@"Tests\CodeDomCommon.box");

            // Checking namespaces.
            Assert.AreEqual(2, codeDom.Namespaces.Count);
            Assert.AreEqual(typeof(Namespace.Default), codeDom.Namespaces[1].Parent.GetType());
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

            // Checking attributes.
            Assert.AreEqual("int", codeDom.Namespaces[0].Models[0].Members[2].ColumnCastingType);
            Assert.AreEqual("ModelsToPersons", codeDom.Namespaces[0].Models[0].TableName);
            Assert.AreEqual("PersonId", codeDom.Namespaces[0].Models[0].Members[1].ColumnName);
            Assert.AreEqual("App.Database", codeDom.Namespaces[1].Models[0].DatabaseRef);

            // Checking data types.
            Assert.AreEqual("int default 0", codeDom.Namespaces[1].Models[0].Members[2].ColumnDataType);
            Assert.AreEqual("varchar(255)", codeDom.Namespaces[1].Models[1].Members[0].ColumnDataType);
            Assert.AreEqual("int", codeDom.Namespaces[1].Models[2].Members[0].ColumnDataType);
        }

        [TestMethod, Priority(10)]
        [Description("Models inheritance test.")]
        public void Inheritance()
        {
            string code, file; CodeDom codeDom;
            Regex re = new Regex(@".+(using Definitif;.+)", RegexOptions.Singleline);

            // Checking single inheritance.
            codeDom = CodeDom.ParseFile(@"Tests\Generator\Inheritance.box");
            code = codeDom.Generate("Definitif.Test");
            file = File.ReadAllText(@"Tests\Generator\Inheritance.result");

            Assert.AreEqual(file, code);

            // And multiple inheritance.
            codeDom = CodeDom.ParseFile(@"Tests\Generator\MultipleInheritance.box");
            code = codeDom.Generate("Definitif.Test");
            file = File.ReadAllText(@"Tests\Generator\MultipleInheritance.result");

            Assert.AreEqual(file, code);
        }

        private void CodeDomGeneratorSnip(string inputFile, string expectedFile, string errorMessage)
        {
            string code, file; CodeDom codeDom;
            string ns = "Definitif.Test";
            string dr = @"Tests\Generator\";
            Regex re = new Regex(@".+(using Definitif;.+)", RegexOptions.Singleline);

            codeDom = CodeDom.ParseFile(dr + inputFile);
            code = codeDom.Generate(ns);
            file = File.ReadAllText(dr + expectedFile);

            // Removing autogenerated header from files.
            code = re.Replace(code, "$1");
            file = re.Replace(file, "$1");

            Assert.AreEqual(file, code, errorMessage);
        }

        [TestMethod, Priority(10)]
        [Description("CodeDom generator test.")]
        public void CodeDomGenerator()
        {
            CodeDomGeneratorSnip("Model.box", "Model.result",
                "Generic model code test failed.");

            CodeDomGeneratorSnip("Body.box", "Body.result",
                "Namespaces and custom bodies test failed.");

            CodeDomGeneratorSnip("Autodoc.box", "Autodoc.result",
                "Autodoc test failed.");

            CodeDomGeneratorSnip("ForeignKey.box", "ForeignKey.result",
                "Foreign keys test failed.");

            CodeDomGeneratorSnip("PrimaryKey.box", "PrimaryKey.result",
                "Primary keys test failed.");

            CodeDomGeneratorSnip("ManyToMany.box", "ManyToMany.result",
                "Many to many model test failed.");
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
                    [""Models""]
                    model Model {
                        namespace InnerNamespace {
                        }
                    }
                }
            }");
        }
    }
}
