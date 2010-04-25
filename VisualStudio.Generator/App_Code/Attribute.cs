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
        /// Parses passed attribute represented as line.
        /// </summary>
        /// <param name="line">Line to parse.</param>
        /// <returns>Parsed attribute dom.</returns>
        public static Attribute Parse(string line)
        {
            string trimmed = line.Trim();
            if (trimmed.IndexOf('"', 2) < 0)
            {
                throw new FormatException("Invalid attribute string format.");
            }

            return new Attribute()
            {
                String = trimmed.Substring(2, trimmed.IndexOf('"', 2) - 2),
                As = (trimmed.IndexOf(" as ") > 0) ?
                    trimmed.Split(new string[] { " as " }, StringSplitOptions.None)[1].Replace("]", "").Trim() : null,
            };
        }
    }
}
