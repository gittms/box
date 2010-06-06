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
        /// Converts Model object to ManyToMany Model CodyType instance collection.
        /// </summary>
        private static CodeTypeDeclaration[] ToManyToManyModelCodeType(this Model model, CodeNamespace mappersNamespace)
        {
            List<CodeTypeDeclaration> result = new List<CodeTypeDeclaration>();

            // Generating generic model codetype.
            CodeTypeDeclaration codeType = model.ToModelCodeType(mappersNamespace)[0], extension;
            codeType.BaseTypes.Add("IManyToMany");
            result.Add(codeType);

            // Many to many relation requires 2 mapped foreign keys.
            Member[] foreignKeys = model.GetForeignKeys();
            if (foreignKeys.Length != 2)
            {
                throw new FormatException("Many to many reletion requires 2 mapped foreign keys members.");
            }

            // Adding methods to extension classes.
            for (int index = 0; index <= 1; index++)
            {
                extension = foreignKeys[index].ToCodeType();
                extension.Members.Add(new CodeSnippetTypeMember(@"
        /// <summary>
        /// Gets linked {type} objects.
        /// </summary>
        public List<ManyToMany<{linkType}, {type}>> Get{type}s() {{
            return ManyToMany<{linkType}, {type}>.Mapper.Read(this.id);
        }}".F(new
           {
               linkType = model.Name,
               type = foreignKeys[1 - index].Type,
           }) + Environment.NewLine)
                );
                result.Add(extension);

                // Breaking loop if foreign keys are of single type.
                if (foreignKeys[0].Type == foreignKeys[1].Type) break;
            }

            return result.ToArray();
        }

        /// <summary>
        /// Gets list of model foreign keys.
        /// </summary>
        private static Member[] GetForeignKeys(this Model model)
        {
            List<Member> foreignKeys = new List<Member>();
            foreach (Member member in model.Members)
            {
                if (member.IsMapped && (member.Modifiers & Modifier.Foreign_key) != 0)
                {
                    foreignKeys.Add(member);
                }
            }

            return foreignKeys.ToArray();
        }

        /// <summary>
        /// Converts Model object to ManyToMany Mapper CodyType instance.
        /// </summary>
        private static CodeTypeDeclaration[] ToManyToManyMapperCodeType(this Model model, CodeNamespace modelsNamespace)
        {
            List<CodeTypeDeclaration> result = new List<CodeTypeDeclaration>();

            // Generating generic mapper codetype.
            CodeTypeDeclaration codeType = model.ToMapperCodeType(modelsNamespace)[0];
            codeType.BaseTypes.Add("IManyToManyMapper");
            result.Add(codeType);

            Member[] foreignKeys = model.GetForeignKeys();

            codeType.Members.Add(new CodeSnippetTypeMember(@"
        public string FieldNameJoin(IModel model) {{
            if (model is {modelsNamespace}.{firstType}) {{
                return ""{firstKey}"";
            }}
            else if (model is {modelsNamespace}.{secondType}) {{
                return ""{secondKey}"";
            }}
            else throw new ArgumentException();
        }}

        public string FieldNameWhere(IModel model) {{
            if (model is {modelsNamespace}.{firstType}) {{
                return ""{secondKey}"";
            }}
            else if (model is {modelsNamespace}.{secondType}) {{
                return ""{firstKey}"";
            }}
            else throw new ArgumentException();
        }}".F(new
            {
                firstType = foreignKeys[0].Type,
                firstKey = foreignKeys[0].ColumnName,
                secondType = foreignKeys[1].Type,
                secondKey = foreignKeys[1].ColumnName,
                modelsNamespace = modelsNamespace.Name,
            }) + Environment.NewLine));

            return result.ToArray();
        }
    }
}
