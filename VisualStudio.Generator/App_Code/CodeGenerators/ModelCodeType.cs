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
        public static CodeTypeDeclaration ToModelCodeType(this Model model, CodeNamespace mappersNamespace)
        {
            CodeTypeDeclaration codeType = model.ToCodeType(true);
            codeType.BaseTypes.Add(String.Format("Model<{0}.{1}>", mappersNamespace.Name, model.Name));

            // Generating list of columns for table scheme.
            List<string> columns = new List<string>();
            foreach (Member member in model.Members)
            {
                var replacement = new {
                    type = member.Type,
                    name = member.Name,
                    column = member.ColumnName,
                    protectedName = member.NameProtected,
                };

                if ((member.Modifiers & Modifier.Foreign_key) != 0)
                {
                    columns.Add(@"private {type}TableScheme {protectedName} = new {type}().C;".F(replacement));
                    columns.Add(@"public {type}TableScheme {name} {{ get {{ return {protectedName}; }} }}".F(replacement));
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
            {columns}
        }}

        private static {type}TableScheme tableScheme = new {type}TableScheme();
        /// <summary>
        /// Gets {type} model table scheme.
        /// </summary>
        public new C {{
            get {{
                return tableScheme;
            }}
        }}".F(new
           {
               type = model.Name,
               mappersNamespace = mappersNamespace.Name,
               columns = String.Join(Environment.NewLine, columns.ToArray()).Indent(3 * 4).Trim(),
           }) + Environment.NewLine));

            foreach (Member member in model.Members)
            {
                // Foreign key member.
                if ((member.Modifiers & Modifier.Foreign_key) != 0)
                {
                    codeType.Members.Add(member.ToForeignKeyMember());
                }
                else
                {
                    codeType.Members.Add(member.ToCodeTypeMember());
                }
            }

            return codeType;
        }
    }
}
