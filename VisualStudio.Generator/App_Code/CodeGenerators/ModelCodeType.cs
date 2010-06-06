using System;
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
    public static partial class Extensions
    {
        /// <summary>
        /// Converts Model object to Model CodeType instance.
        /// </summary>
        public static CodeTypeDeclaration[] ToModelCodeType(this Model model, CodeNamespace mappersNamespace)
        {
            List<CodeTypeDeclaration> result = new List<CodeTypeDeclaration>();

            CodeTypeDeclaration codeType = model.ToCodeType(true);
            codeType.BaseTypes.Add(String.Format("Model<{0}.{1}>", mappersNamespace.Name, model.Name));
            result.Add(codeType);

            // Generating list of columns for table scheme.
            List<string> columns = new List<string>(),
                schemeConstructor = new List<string>();
            foreach (Member member in model.Members)
            {
                var replacement = new {
                    type = member.Type,
                    typeSafe = member.Type.Replace('.', '_'),
                    name = member.Name,
                    column = member.ColumnName,
                    protectedName = member.NameProtected,
                };

                if ((member.Modifiers & Modifier.Foreign_key) != 0)
                {
                    columns.Add(@"private {type}.{typeSafe}TableScheme {protectedName} = new {type}().C;".F(replacement));
                    columns.Add(@"public {type}.{typeSafe}TableScheme {name} {{ get {{ return {protectedName}; }} }}".F(replacement));
                    schemeConstructor.Add((@"if (!{protectedName}.Id.ForeignKeys.Contains(table[""{column}""])) " +
                        @"{protectedName}.Id.ForeignKeys.Add(table[""{column}""]);").F(replacement));
                }
                else if (member.IsMapped)
                {
                    columns.Add(@"private Column {protectedName} = table[""{column}""];".F(replacement));
                    columns.Add(@"public Column {name} {{ get {{ return {protectedName}; }} }}".F(replacement));
                }
            }

            // Generating common header.
            codeType.Members.Add(new CodeSnippetTypeMember(@"
        /// <summary>
        /// Gets {type} instance from database by given Id.
        /// </summary>
        public static {type} Get(Id id) {{
            return {type}.Mapper.Read(id);
        }}

        public class {type}TableScheme : ModelTableScheme<{mappersNamespace}.{type}> {{
            public {type}TableScheme() {{
                {schemeConstructor}
            }}

            {columns}
        }}

        private static {type}TableScheme tableScheme = new {type}TableScheme();
        /// <summary>
        /// Gets {type} model table scheme.
        /// </summary>
        public new {type}TableScheme C {{
            get {{
                return tableScheme;
            }}
        }}".F(new
           {
               type = model.Name,
               mappersNamespace = mappersNamespace.Name,
               columns = String.Join(Environment.NewLine, columns.ToArray()).Indent(3 * 4).Trim(),
               schemeConstructor = String.Join(Environment.NewLine, schemeConstructor.ToArray()).Indent(4 * 4).Trim(),
           }) + Environment.NewLine));

            CodeTypeDeclaration extension;
            foreach (Member member in model.Members)
            {
                // Foreign key member.
                if ((member.Modifiers & Modifier.Foreign_key) != 0)
                {
                    extension = member.ToCodeType();
                    extension.Members.Add(new CodeSnippetTypeMember(@"
        /// <summary>
        /// Gets linked {model} objects.
        /// </summary>
        public {model}[] Get{model}s() {{
            return new Select<{model}>().Where(m => m.C.{member}.Id == this.Id).Read();
        }}".F(new
           {
               model = model.Name,
               member = member.Name,
           }) + Environment.NewLine)
                );
                    result.Add(extension);
                    codeType.Members.Add(member.ToForeignKeyMember());
                }
                else
                {
                    codeType.Members.Add(member.ToCodeTypeMember());
                }

                // Primary key member.
                if ((member.Modifiers & Modifier.Primary_key) != 0)
                {
                    codeType.Members.Add(new CodeSnippetTypeMember(@"
        /// <summary>
        /// Gets {model} instance from database by given {member}.
        /// </summary>
        public static {model} GetBy{member}({memberType} {param}) {{
            return new Select<{model}>().Where(m => m.C.{member} == {param}).ReadFirst();
        }}".F(new
           {
               model = model.Name,
               member = member.Name,
               memberType = member.Type,
               param = member.Name.ToLower(),
           }) + Environment.NewLine));
                }
            }

            return result.ToArray();
        }
    }
}
