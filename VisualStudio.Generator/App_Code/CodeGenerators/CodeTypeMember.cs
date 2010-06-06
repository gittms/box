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
        /// Converts Member instance to CodeSnippet.
        /// </summary>
        public static CodeTypeMember ToCodeTypeMember(this Member member)
        {
            if (member.IsMapped)
            {
                return member.ToProperty();
            }
            else if (member.MemberBody != null)
            {
                return member.ToCustomMember();
            }
            else
            {
                return new CodeSnippetTypeMember();
            }
        }
        /// <summary>
        /// Converts Member instance to CodeSnippet:
        ///     protected {type} {name};
        ///     {modifier} {type} {Name} {
        ///         get { .. }; set { .. };
        ///     }
        /// </summary>
        private static CodeTypeMember ToProperty(this Member member)
        {
            CodeSnippetTypeMember property = new CodeSnippetTypeMember(@"
        [DataMember(Name = ""{name}"", IsRequired = false)]
        {modifiers} {type} {name} {{
            get {{ return {protectedName}; }}
            set {{ {protectedName} = value; }}
        }}
        protected {type} {protectedName};".F(new {
                type = member.Type,
                name = member.Name,
                protectedName = member.NameProtected,
                modifiers = member.Modifiers.ToString(true)
            }) + Environment.NewLine);
            property.AddAutodoc(member.Autodoc);

            return property;
        }
        /// <summary>
        /// Converts Member instance to one of CodeSnippets:
        ///     {modifier} {type} {Name} = {value};
        ///     {modifier} {type} {Name} { ... }
        ///     {modifier} {type} {Name}(args) { ... }
        /// </summary>
        private static CodeTypeMember ToCustomMember(this Member member)
        {
            CodeSnippetTypeMember custom = new CodeSnippetTypeMember(@"
        {modifiers} {type} {name} {body}".F(new {
                type = member.Type,
                name = member.Name,
                modifiers = member.Modifiers.ToString(true),
                body = member.MemberBody.Indent(2 * 4).Trim()
            }) + Environment.NewLine);
            custom.AddAutodoc(member.Autodoc);

            return custom;
        }

        /// <summary>
        /// Converts Member instance to CodeSnippet:
        ///     protected Id {name}Id { get; set; }
        ///     {modifier} {type} {name} { get; set; }
        /// </summary>
        public static CodeTypeMember ToForeignKeyMember(this Member member)
        {
            CodeSnippetTypeMember foreignKey = new CodeSnippetTypeMember(@"
        [DataMember(Name = ""{name}"", IsRequired = false)]
        private Int64 {name}IdSurrogate {{
            get {{ return (Int64){idName}.Value; }}
            set {{ this.{idName} = new Id(value); }}
        }}

        internal Id {name}Id {{
            get {{ return {idName}; }}
            set {{ {idName} = value; }}
        }}
        protected Id {idName} = Id.Empty;

        {modifiers} {type} {name} {{
            get {{
                if ({protectedName} == null && {idName} != Id.Empty) {{
                    {protectedName} = {type}.Get({idName});
                }}
                return {protectedName};
            }}
            set {{
                {protectedName} = value;
                {idName} = value.Id;
            }}
        }}
        protected {type} {protectedName} = null;".F(new {
                name = member.Name,
                idName = "id_" + member.Name.ToLower(),
                modifiers = member.Modifiers.ToString(true),
                type = member.Type,
                protectedName = member.NameProtected
            }) + Environment.NewLine);
            foreignKey.AddAutodoc(member.Autodoc);

            return foreignKey;
        }
    }
}
