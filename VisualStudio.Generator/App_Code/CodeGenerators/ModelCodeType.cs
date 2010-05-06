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

            codeType.Members.Add(new CodeSnippetTypeMember(@"
        /// <summary>
        /// Gets {type} instance from database by given Id.
        /// </summary>
        public static {type} Get(Id id) {{
            return {type}.Mapper.Read(id);
        }}".F(new
           {
               type = model.Name
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
