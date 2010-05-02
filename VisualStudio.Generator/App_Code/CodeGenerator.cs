﻿using System;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Definitif;

namespace Definitif.VisualStudio.Generator
{
    /// <summary>
    /// Represents Generator Constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Default database reference string.
        /// </summary>
        public static string DefaultDatabase { get { return "Core.Database"; } }
    }

    /// <summary>
    /// Represents code generator for CodeDom.
    /// </summary>
    internal class CodeGenerator
    {
        /// <summary>
        /// Generates code for given CodeDom instance.
        /// </summary>
        /// <param name="dom">CodeDom to generate code for.</param>
        /// <returns>String containing source code.</returns>
        public static string Generate(CodeDom dom, string defaultNamespace)
        {
            CodeCompileUnit code = new CodeCompileUnit();

            // Importing common namespaces.
            code.Namespaces.Add(new CodeNamespace()
            {
                Imports = {
                    new CodeNamespaceImport("System"),
                    new CodeNamespaceImport("System.Collections.Generic"),
                    new CodeNamespaceImport("System.Data"),
                    new CodeNamespaceImport("Definitif.Data"),
                    new CodeNamespaceImport("Definitif.Data.ObjectSql"),
                    new CodeNamespaceImport("Definitif.Data.ObjectSql.Query"),
                },
            });

            // Generating code for namespaces.
            foreach (Namespace ns in dom.Namespaces)
            {
                // Skipping empty namespaces.
                if (ns.Models.Count == 0) continue;

                // Setting name for default one.
                if (ns is Namespace.Default)
                {
                    ns.Name = defaultNamespace;
                }

                // Adding generated namespaces.
                code.Namespaces.AddRange(ns.ToCodeNamespaces());
            }

            // Formatting and outputing code.
            StringWriter writer = new StringWriter();
            new CSharpCodeProvider().GenerateCodeFromCompileUnit(
                compileUnit : code,
                writer : writer,
                options: new CodeGeneratorOptions()
                {
                    IndentString = "    ",
                    BlankLinesBetweenMembers = false,
                });

            return writer.ToString();
        }
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Converts Namespace object to CodeNamespace with imports.
        /// </summary>
        /// <returns>CodeNamespace instance.</returns>
        public static CodeNamespace[] ToCodeNamespaces(this Namespace ns)
        {
            CodeNamespace modelsNamespace = new CodeNamespace(ns.Name),
                mappersNamespace = new CodeNamespace(ns.Name + ".Mappers");

            foreach (Model model in ns.Models)
            {
                // Many to many model.
                if ((model.Modifiers & Modifier.Many_to_many) != 0)
                {
                    modelsNamespace.Types.Add(model.ToManyToManyModelCodeType(mappersNamespace));
                    mappersNamespace.Types.Add(model.ToManyToManyMapperCodeType(modelsNamespace));
                }
                else
                {
                    modelsNamespace.Types.Add(model.ToModelCodeType(mappersNamespace));
                    mappersNamespace.Types.Add(model.ToMapperCodeType(modelsNamespace));
                }
            }

            return new CodeNamespace[] {
                modelsNamespace,
                mappersNamespace,
            };
        }

        /// <summary>
        /// Converts Modifier value to related TypeAttributes one.
        /// </summary>
        /// <returns>TypeAttributes value.</returns>
        public static TypeAttributes ToTypeAttributes(this Modifier modifiers)
        {
            if ((modifiers & Modifier.Private) != 0) return TypeAttributes.NestedPrivate;
            else if ((modifiers & Modifier.Internal) != 0) return TypeAttributes.NotPublic;
            else return TypeAttributes.Public;
        }

        /// <summary>
        /// Converts Model object to common CodeType instance.
        /// </summary>
        public static CodeTypeDeclaration ToCodeType(this Model model, bool withAutodoc)
        {
            // Creating declaration.
            CodeTypeDeclaration codeType = new CodeTypeDeclaration(model.Name)
            {
                IsClass = true,
                IsPartial = true,
            };
            if (withAutodoc) codeType.AddAutodoc(model.Autodoc);
            codeType.TypeAttributes = model.Modifiers.ToTypeAttributes();

            return codeType;
        }

        /// <summary>
        /// Converts Model object to ManyToMany Model CodyType instance.
        /// </summary>
        private static CodeTypeDeclaration ToManyToManyModelCodeType(this Model model, CodeNamespace mappersNamespace)
        {
            // Generating generic model codetype.
            CodeTypeDeclaration codeType = model.ToModelCodeType(mappersNamespace);

            return codeType;
        }

        /// <summary>
        /// Converts Model object to ManyToMany Mapper CodyType instance.
        /// </summary>
        private static CodeTypeDeclaration ToManyToManyMapperCodeType(this Model model, CodeNamespace modelsNamespace)
        {
            // Generating generic mapper codetype.
            CodeTypeDeclaration codeType = model.ToMapperCodeType(modelsNamespace);

            return codeType;
        }
    }
}
