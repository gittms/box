using System;

namespace Definitif.Box.Unbox
{
    partial class Program
    {
        static string nl = Environment.NewLine;
        static ConsoleColor defaultColor = Console.ForegroundColor;
        static int indent = 0;

        /// <summary>
        /// Writes message to console.
        /// </summary>
        private static void W(string line)
        {
            W(line, "");
        }
        /// <summary>
        /// Writes color message to console.
        /// </summary>
        private static void WC(string line, ConsoleColor color)
        {
            WC(line, color, "");
        }

        /// <summary>
        /// Writes String.Format() message to console.
        /// </summary>
        private static void W(string line, params object[] args)
        {
            string indentStr = "".PadLeft(indent, ' ');
            line = indentStr + line.Replace(nl, nl + indentStr);
            Console.WriteLine(line, args);
        }
        /// <summary>
        /// Writes color String.Format() message to console.
        /// </summary>
        private static void WC(string line, ConsoleColor color, params object[] args)
        {
            Console.ForegroundColor = color;
            W(line, args);
            Console.ForegroundColor = defaultColor;
        }

        /// <summary>
        /// Indents output by 4 spaces.
        /// </summary>
        private static void WIndent()
        {
            indent += 4;
        }
        /// <summary>
        /// Unindents output by 4 spaces.
        /// </summary>
        private static void WUnindent()
        {
            indent -= 4;
            if (indent < 0) indent = 0;
        }
    }
}
