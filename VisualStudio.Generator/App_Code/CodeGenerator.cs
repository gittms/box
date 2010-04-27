using System;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;

namespace Definitif.VisualStudio.Generator
{
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
                    new CodeNamespaceImport("System.Data"),
                    new CodeNamespaceImport("Definitif.Data"),
                    new CodeNamespaceImport("Definitif.Data.ObjectSql"),
                },
            });

            // Generating code for namespaces.
            foreach (Namespace ns in dom.Namespaces)
            {
                if (ns.Models.Count == 0) continue;

                if (ns is Namespace.Default)
                {
                    ns.Name = defaultNamespace;
                }

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
                modelsNamespace.Types.Add(model.ToModelCodeType(mappersNamespace));
                mappersNamespace.Types.Add(model.ToMapperCodeType(modelsNamespace));
            }

            return new CodeNamespace[] {
                modelsNamespace,
                mappersNamespace,
            };
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

            // Adding parameters.
            if ((model.Modifiers & Modifier.Private) != 0) codeType.TypeAttributes = TypeAttributes.NestedPrivate;
            else if ((model.Modifiers & Modifier.Internal) != 0) codeType.TypeAttributes = TypeAttributes.NotPublic;
            else codeType.TypeAttributes = TypeAttributes.Public;

            // Adding autodoc.
            if (withAutodoc && !String.IsNullOrEmpty(model.Autodoc))
            {
                codeType.Comments.Add(new CodeCommentStatement(new CodeComment(model.Autodoc, true)));
            }

            return codeType;
        }

        /// <summary>
        /// Gets model mapped members list.
        /// </summary>
        public static Member[] GetMappedMembers(this Model model)
        {
            return model.Members.Where(member => member.ColumnName != null).ToArray();
        }

        /// <summary>
        /// Converts Model object to Model CodeType instance.
        /// </summary>
        public static CodeTypeDeclaration ToModelCodeType(this Model model, CodeNamespace mappersNamespace)
        {
            if ((model.Modifiers & Modifier.Many_to_many) != 0) return model.ToManyToManyModelCodeType(mappersNamespace);
            CodeTypeDeclaration codeType = model.ToCodeType(true);
            codeType.BaseTypes.Add(String.Format("Model<{0}.{1}>", mappersNamespace.Name, model.Name));

            return codeType;
        }

        /// <summary>
        /// Converts Model object to ManyToMany Model CodyType instance.
        /// </summary>
        private static CodeTypeDeclaration ToManyToManyModelCodeType(this Model model, CodeNamespace mappersNamespace)
        {
            return new CodeTypeDeclaration();
        }

        /// <summary>
        /// Converts Model object to Mapper CodeType instance.
        /// </summary>
        public static CodeTypeDeclaration ToMapperCodeType(this Model model, CodeNamespace modelsNamespace)
        {
            if ((model.Modifiers & Modifier.Many_to_many) != 0) return model.ToManyToManyMapperCodeType(modelsNamespace);
            CodeTypeDeclaration codeType = model.ToCodeType(false);
            codeType.BaseTypes.Add(String.Format("Mapper<{0}.{1}>", modelsNamespace.Name, model.Name));

            return codeType;
        }

        /// <summary>
        /// Converts Model object to ManyToMany Mapper CodyType instance.
        /// </summary>
        private static CodeTypeDeclaration ToManyToManyMapperCodeType(this Model model, CodeNamespace modelsNamespace)
        {
            return new CodeTypeDeclaration();
        }
    }
}
