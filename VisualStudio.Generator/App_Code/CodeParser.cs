using System;
using System.Linq;
using System.Collections.Generic;

namespace Definitif.VisualStudio.Generator
{
    /// <summary>
    /// Represents parser for CodeDom.
    /// </summary>
    internal class CodeParser
    {
        private class Context
        {
            public List<object> Brackets { get; set; }
            public Namespace CurrentNamespace { get; set; }
            public Model CurrentModel { get; set; }
            public Member CurrentMember { get; set; }
            public List<string> Autodoc { get; set; }
            public string Attribute { get; set; }

            public Context()
            {
                this.Brackets = new List<object>();
                this.Autodoc = new List<string>();
            }
        }

        public static void Parse(CodeDom dom, string input)
        {
            Namespace nspace = new Namespace.Default();
            dom.Namespaces = new List<Namespace>() { nspace };

            Context context = new Context();
            context.CurrentNamespace = nspace;

            foreach (string line in input.Split('\n'))
            {
                CodeParser.ParseLine(dom, line, context);
            }
        }

        private static void ParseLine(CodeDom dom, string line, Context context)
        {
            string trimmed = line.Trim();

            // Comments and autodoc.
            if (trimmed.StartsWith("///"))
            {
                context.Autodoc.Add(trimmed);
                return;
            }
            if (trimmed.StartsWith("//")) return;

            // Attribute block.
            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                context.Attribute = trimmed;
                return;
            }

            // Namespace declaration.
            if (trimmed.StartsWith("namespace"))
            {
                Namespace nspace = new Namespace()
                {
                    Name = trimmed.Split(' ')[1],
                    ParentNamespace = context.CurrentNamespace,
                };
                // Composing names if it's not a default namespace child.
                if (context.Brackets.Count > 0)
                {
                    if (!(context.Brackets.Last() is Namespace))
                    {
                        throw new FormatException("Namespace can only be placed inside another namespace.");
                    }
                    nspace.Name = context.CurrentNamespace.Name + "." + nspace.Name;
                }
                dom.Namespaces.Add(nspace);
                context.Brackets.Add(nspace);
                context.CurrentNamespace = nspace;
                return;
            }

            // Model declaration.
            if (context.CurrentModel == null && trimmed.Contains("model"))
            {
                context.CurrentModel = new Model()
                {
                    Autodoc = String.Join(Environment.NewLine, context.Autodoc.ToArray()),
                    Name = trimmed.Split(' ').Last(),
                    Modifiers = Modifier.Default.Parse(trimmed),
                };

                // Parsing attributes.
                if (String.IsNullOrWhiteSpace(context.Attribute))
                {
                    throw new FormatException("Model requires proper attribute declaration.");
                }
                Attribute attr = Attribute.Parse(context.Attribute);

                // Clearing autodoc and saving refs.
                context.CurrentNamespace.Models.Add(context.CurrentModel);
                context.Brackets.Add(context.CurrentModel);
                context.Autodoc.Clear();
                context.Attribute = null;
                return;
            }

            // Model members declaration.
            if (context.CurrentModel != null && context.CurrentMember == null)
            {
                context.CurrentMember = new Member()
                {
                    Modifiers = Modifier.Default.Parse(trimmed),
                };
                context.CurrentModel.Members.Add(context.CurrentMember);

                Attribute attr = null;
                if (!String.IsNullOrWhiteSpace(context.Attribute))
                {
                    attr = Attribute.Parse(context.Attribute);
                    context.CurrentMember.ColumnName = attr.String;
                    context.CurrentMember.ColumnCastingType = attr.As;
                }

                // Getting member name and body.
                if (trimmed.Contains("{"))
                {
                    string[] parts = trimmed.Split('{')[0].Split(' ');
                    context.CurrentMember.Name = parts[parts.Length - 1];

                    context.CurrentMember.MemberBody = "{" + trimmed.Split(new char[] { '{' }, 2)[1];

                    if (trimmed.EndsWith("}"))
                    {
                        context.CurrentMember = null;
                        return;
                    }
                }
                else if (trimmed.Contains("="))
                {
                    string[] parts = trimmed.Split('=')[0].Split(' ');
                    context.CurrentMember.Name = parts[parts.Length - 1];

                    context.CurrentMember.MemberBody = "=" + trimmed.Split(new char[] { '=' }, 2)[1];

                    if (trimmed.EndsWith(";"))
                    {
                        context.CurrentMember = null;
                        return;
                    }
                }
            }
            // Model members body.
            else if (context.CurrentModel != null && context.CurrentMember != null)
            {
                context.CurrentMember.MemberBody += Environment.NewLine + trimmed;
                if (trimmed.EndsWith("}") || trimmed.EndsWith(";"))
                {
                    context.CurrentMember = null;
                }
                return;
            }

            // Closing bracket.
            if (trimmed.EndsWith("}"))
            {
                if (context.Brackets.Count == 0)
                {
                    throw new FormatException("Closing bracket mismatches opening ones.");
                }

                // If bracket closes namespace..
                if (context.Brackets.Last() is Namespace)
                {
                    context.CurrentNamespace = context.CurrentNamespace.ParentNamespace;
                }
                // ..or model declaration..
                else if (context.Brackets.Last() is Model)
                {
                    context.CurrentModel = null;
                }
                context.Brackets.RemoveLast();
                return;
            }
        }
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Removes last item from List.
        /// </summary>
        public static void RemoveLast(this List<object> list)
        {
            list.RemoveAt(list.Count - 1);
        }
    }
}
