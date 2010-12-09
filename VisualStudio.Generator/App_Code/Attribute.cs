using System;

namespace Definitif.VisualStudio.Generator
{
    /// <summary>
    /// Represents attribute model.
    /// </summary>
    internal class Attribute
    {
        /// <summary>
        /// Gets or sets string value from attribute.
        /// </summary>
        public string String { get; set; }
        /// <summary>
        /// Gets or sets as value from attribute.
        /// </summary>
        public string As { get; set; }
        /// <summary>
        /// Gets or sets in value from attribute.
        /// </summary>
        public string In { get; set; }
        /// <summary>
        /// Gets or sets custom type from attribute.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Parses passed attribute represented as line.
        /// </summary>
        /// <param name="line">Line to parse.</param>
        /// <returns>Parsed attribute dom.</returns>
        public static Attribute Parse(string line)
        {
            string trimmed = line.Trim();
            string left = trimmed, right = "";
            if (trimmed.Contains(","))
            {
                left = trimmed.Split(',')[0].Trim();
                right = trimmed.Split(',')[1].Replace('"', ' ').Trim();
            }

            if (trimmed.IndexOf('"', 2) < 0)
            {
                throw new FormatException("Invalid attribute string format.");
            }

            return new Attribute()
            {
                String = trimmed.Substring(2, trimmed.IndexOf('"', 2) - 2),
                As = (trimmed.IndexOf(" as ") > 0) ?
                    left.Split(new string[] { " as " }, StringSplitOptions.None)[1].Replace("]", "").Trim() : null,
                In = (trimmed.IndexOf(" in ") > 0) ?
                    left.Split(new string[] { " in " }, StringSplitOptions.None)[1].Replace("]", "").Trim() : null,
                Type = (right != "") ? right.Replace("]", "").Trim() : null,
            };
        }
    }
}
