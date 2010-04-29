using System;
using System.Linq;
using Definitif.Box;

namespace Definitif
{
    public static class StringEntensions
    {
        /// <summary>
        /// Formats string using provided data dynamic object.
        /// </summary>
        /// <returns>Formatted string.</returns>
        public static String F(this String str, object data)
        {
            return NamedFormat.FormatWith(str, data);
        }

        /// <summary>
        /// Indents string by given number of spaces.
        /// </summary>
        /// <returns>Indented string.</returns>
        public static string Indent(this string str, int spaces)
        {
            string indent = "".PadLeft(spaces),
                result = "";

            foreach (string line in str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (result != "") result += Environment.NewLine;
                result += indent + line;
            }

            return result;
        }
    }
}
