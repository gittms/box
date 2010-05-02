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
        /// Trimms origin autodoc string and return ready for CodeComment generation string.
        /// </summary>
        public static string TrimAutodoc(this string autodoc)
        {
            string result = "";
            foreach (string line in autodoc.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (result != "")
                {
                    result += Environment.NewLine + line.Replace("///", "");
                }
                else
                {
                    result += line.Replace("///", "").Trim();
                }
            }
            return result;
        }
        /// <summary>
        /// Adds autodoc block to CodeTypeMember.
        /// </summary>
        /// <param name="autodoc">Autodoc string to add.</param>
        public static void AddAutodoc(this CodeTypeMember member, string autodoc)
        {
            if (!String.IsNullOrWhiteSpace(autodoc))
            {
                if (member is CodeSnippetTypeMember)
                {
                    CodeSnippetTypeMember memb = (CodeSnippetTypeMember)member;
                    memb.Text = autodoc.Indent(2 * 4) + memb.Text;
                }
                else
                {
                    member.Comments.Add(new CodeCommentStatement(new CodeComment(autodoc.TrimAutodoc(), true)));
                }
            }
        }
        /// <summary>
        /// Adds autodoc block to CodeTypeDeclaration.
        /// </summary>
        /// <param name="autodoc">Autodoc string to add.</param>
        public static void AddAutodoc(this CodeTypeDeclaration codeType, string autodoc)
        {
            if (!String.IsNullOrWhiteSpace(autodoc))
            {
                codeType.Comments.Add(new CodeCommentStatement(new CodeComment(autodoc.TrimAutodoc(), true)));
            }
        }
    }
}
