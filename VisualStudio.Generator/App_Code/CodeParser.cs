using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Re = System.Text.RegularExpressions.Regex;

namespace Definitif.VisualStudio.Generator
{
    /// <summary>
    /// Represents parser for CodeDom.
    /// </summary>
    internal class CodeParser
    {
        /// <summary>
        /// Represents CodeDom parsing context.
        /// </summary>
        private class Context
        {
            public List<object> Brackets { get; set; }
            public Namespace CurrentNamespace { get; set; }
            public Model CurrentModel { get; set; }
            public Member CurrentMember { get; set; }
            public List<string> Autodoc { get; set; }
            public string Attribute { get; set; }

            /// <summary>
            /// Creates an empty parsing context.
            /// </summary>
            public Context()
            {
                this.Brackets = new List<object>();
                this.Autodoc = new List<string>();
            }
        }

        /// <summary>
        /// Parses given input into CodeDom instance.
        /// </summary>
        /// <param name="dom">CodeDom to parse into.</param>
        /// <param name="input">Input string to parse.</param>
        public static void Parse(CodeDom dom, string input)
        {
            Namespace nspace = new Namespace.Default();
            dom.Namespaces = new List<Namespace>() { nspace };

            Context context = new Context();
            context.CurrentNamespace = nspace;

            int lineNumber = 1;
            foreach (string line in input.Split('\n'))
            {
                try
                {
                    CodeParser.ParseLine(dom, line, context);
                    lineNumber++;
                }
                catch (FormatException inner)
                {
                    // Generating wrapped exception for every
                    // catched FormatException.
                    // Wrapped exception contains current line number.
                    throw new FormatException("'" + inner.Message + "' at line " + lineNumber.ToString(), inner);
                }
            }
        }

        /// <summary>
        /// Parses single input line using parsing contexst.
        /// </summary>
        /// <param name="dom">CodeDom instance to parse into.</param>
        /// <param name="line">Line to parse.</param>
        /// <param name="context">Context to use.</param>
        private static void ParseLine(CodeDom dom, string line, Context context)
        {
            string trimmed = line.Trim();

            // Commented blocks - generic comments and autodoc.
            // Comments can be completely ignored, and autodocs
            // should be appended to context store.
            if (Re.IsMatch(trimmed, "^\\/\\/"))
            {
                if (Re.IsMatch(trimmed, "^\\/\\/\\/"))
                {
                    context.Autodoc.Add(trimmed);
                }
                return;
            }

            // Attributes blocks - lines before models or
            // members declarations.
            // Attributes are placed inside square brackets.
            // TODO: Models and members can have multiple attributes.
            if (Re.IsMatch(trimmed, "^\\[.+\\]$"))
            {
                context.Attribute = trimmed;
                return;
            }

            // Namespaces declarations - lines started with
            // namespace keyword.
            // Namespaces can be declared in root of code document
            // or inside other namespace.
            if (Re.IsMatch(trimmed, "^namespace"))
            {
                Namespace nspace = new Namespace()
                {
                    Name = trimmed.Split(' ').Last(),
                    Parent = context.CurrentNamespace,
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

            // Models declarations - declarations with model
            // keyword.
            // Models can not be declared inside other models.
            if (context.CurrentModel == null && Re.IsMatch(trimmed, "(^|\\s)model(\\s)"))
            {
                // Parsing attributes.
                if (String.IsNullOrWhiteSpace(context.Attribute))
                {
                    throw new FormatException("Model requires proper attribute declaration.");
                }
                Attribute attr = Attribute.Parse(context.Attribute);

                context.CurrentModel = new Model()
                {
                    Autodoc = String.Join(Environment.NewLine, context.Autodoc.ToArray()),
                    Name = trimmed.Split(' ').Last(),
                    TableName = attr.String,
                    Modifiers = Modifier.Default.Parse(trimmed),
                };

                // Clearing autodoc and saving refs.
                context.CurrentNamespace.Models.Add(context.CurrentModel);
                context.Brackets.Add(context.CurrentModel);
                context.Autodoc.Clear();
                context.Attribute = null;
                return;
            }

            // Model members declarations - declarations inside model
            // description.
            if (context.CurrentModel != null && context.CurrentMember == null &&
                !trimmed.In("", "{", "}"))
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

                // Getting member name.
                Match match = Re.Match(trimmed, "(?<name>[A-Za-z0-9]+)(?<body>\\s*(;|=|\\{|$).*)");
                if (!match.Success)
                {
                    throw new FormatException("Model member declaration can not be parsed.");
                }
                context.CurrentMember.Name = match.Groups["name"].Value;

                // If member has no body declaration, going further.
                if (match.Groups[1].Value == ";")
                {
                    context.CurrentMember = null;
                    return;
                }
                // If member declaration ends with newline skipping current
                // body begining to make it easier to manipulate with.
                else if (match.Groups[1].Value == "")
                {
                    return;
                }

                // Otherwise, getting beginning of model member's body.
                string body = match.Groups["body"].Value;
                context.CurrentMember.MemberBody = match.Groups["body"].Value;
                if (match.Groups[1].Value == "=" &&
                    Re.IsMatch(trimmed, ";$"))
                {
                    context.CurrentMember = null;
                    return;
                }
                else if (match.Groups[1].Value == "{" &&
                    body.Count('{') == body.Count('}'))
                {
                    context.CurrentMember = null;
                    return;
                }
            }
            // Model members body.
            else if (context.CurrentModel != null && context.CurrentMember != null)
            {
                context.CurrentMember.MemberBody += Environment.NewLine + trimmed;
                string body = context.CurrentMember.MemberBody;
                if (Re.IsMatch(body, "^\\s*=") &&
                    Re.IsMatch(body, ";$"))
                {
                    context.CurrentMember = null;
                }
                else if (Re.IsMatch(body, "^\\s*{") &&
                    body.Count('{') == body.Count('}'))
                {
                    context.CurrentMember = null;
                }
                return;
            }

            // Closing bracket.
            if (Re.IsMatch(trimmed, "\\}$"))
            {
                if (context.Brackets.Count == 0)
                {
                    throw new FormatException("Closing brackets mismatch opening ones.");
                }

                // If bracket closes namespace..
                if (context.Brackets.Last() is Namespace)
                {
                    context.CurrentNamespace = context.CurrentNamespace.Parent;
                }
                // ..or model declaration..
                else if (context.Brackets.Last() is Model)
                {
                    context.CurrentModel = null;
                }
                // ..or model member body.
                else if (context.Brackets.Last() is Member)
                {
                    context.CurrentMember = null;
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

        /// <summary>
        /// Counts number of character occurences in string.
        /// </summary>
        /// <param name="ch">Char to calculates occurences of.</param>
        /// <returns>Number of char occurences.</returns>
        public static int Count(this String str, char ch)
        {
            int result = 0;

            foreach (char element in str)
            {
                if (element == ch) result++;
            }

            return result;
        }

        /// <summary>
        /// Checks if string is in given params list.
        /// </summary>
        /// <param name="strings">Params list to check string.</param>
        /// <returns>True if at least one of params equals</returns>
        public static bool In(this String str, params string[] strings)
        {
            foreach (string element in strings)
            {
                if (str == element) return true;
            }
            return false;
        }
    }
}
