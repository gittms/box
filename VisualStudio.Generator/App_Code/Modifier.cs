using System;
using System.Collections.Generic;

namespace Definitif.VisualStudio.Generator
{
    /// <summary>
    /// Represents model or member modifiers.
    /// </summary>
    public enum Modifier : int
    {
        Default      = 0,
        Private      = 1,
        Protected    = 2,
        Public       = 4,
        Internal     = 8,
        Static       = 16,
        Private_key  = 32,
        Foreign_key  = 64,
        Many_to_many = 128,
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Converts Modifier to string representation.
        /// </summary>
        /// <returns>String representation of Modifier.</returns>
        public static string ToString(this Modifier modifiers)
        {
            if (modifiers == Modifier.Default) return Modifier.Public.ToString().ToLower();
            List<string> variants = new List<string>();

            foreach (int value in Enum.GetValues(typeof(Modifier)))
            {
                Modifier modifier = (Modifier)value;
                if ((modifiers & modifier) != 0) variants.Add(modifier.ToString().Replace('_', ' ').ToLower());
            }

            return String.Join(" ", variants.ToArray());
        }

        /// <summary>
        /// Perses string representation to Modifier enumerator.
        /// </summary>
        /// <param name="str">String representation of Modifier.</param>
        /// <returns>Modifier value.</returns>
        public static Modifier Parse(this Modifier modifiers, string str)
        {
            if (String.IsNullOrWhiteSpace(str)) return Modifier.Default;

            // Preparing string by replacing some misc substrings.
            str = str.Replace(" key", "_key");

            int result = 0;
            string[] parts = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                Modifier modifier = Modifier.Default;
                if (Enum.TryParse<Modifier>(part.Capitalize(), out modifier))
                {
                    result += (int)modifier;
                }
            }

            return (Modifier)result;
        }

        /// <summary>
        /// Capitalized given string.
        /// </summary>
        /// <returns>Capitalized string.</returns>
        public static string Capitalize(this string str)
        {
            if (String.IsNullOrWhiteSpace(str)) return "";
            return Char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}
