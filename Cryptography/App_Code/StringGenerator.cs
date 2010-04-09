using System;
using System.Text.RegularExpressions;

namespace Definitif.Security.Cryptography
{
    /// <summary>
    /// Represents pattern-based string generator.
    /// </summary>
    public class StringGenerator
    {
        private object syncRoot = new object();
        private Random random;

        /// <summary>
        /// Gets new singleton or sets existing global randomizer.
        /// </summary>
        public Random Random
        {
            get
            {
                if (random == null)
                {
                    lock (syncRoot)
                    {
                        if (random == null)
                        {
                            random = new Random();
                        }
                    }
                }
                return random;
            }
            set { random = value; }
        }

        private const string az = "abcdefghijklmnopqrstuvwxyz",
                             AZ = "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
                             nm = "0123456789";

        /// <summary>
        /// Generates substring represented by definition, i.e.
        /// collection of available symbols, or symbol groups,
        /// like in Regexp, for example "a-zA-Z0-9:\.".
        /// </summary>
        /// <param name="definition">Definition to use for generation.</param>
        /// <param name="length">Length of string to generate.</param>
        /// <returns>Generated string.</returns>
        private string GenerateSubstring(string definition, int length)
        {
            string result = "";
            definition = definition.Replace("a-z", az)
                                   .Replace("A-Z", AZ)
                                   .Replace("0-9", nm)
                                   .Replace("\\", "");

            int len = definition.Length;
            for (int i = 0; i < length; i++)
            {
                result += definition[this.Random.Next(0, len)];
            }

            return result;
        }

        /// <summary>
        /// Generates string using given Regex-based pattern.
        /// Example: [0-9a-z]{2}-[A-Z]{12}@[012]
        /// </summary>
        /// <param name="pattern">Pattern to use for string generation.</param>
        /// <returns>String generated using pattern.</returns>
        public string Generate(string pattern)
        {
            string result = "", definition, length;
            int len, i = 0;

            while (i < pattern.Length)
            {
                // Searching for Regex group.
                if (pattern[i] == '[')
                {
                    // Working with group, getting
                    // length, defintion and preparing
                    // for substring generation.
                    i++; definition = ""; length = "";
                    while (true)
                    {
                        if (pattern[i] == ']')
                        {
                            // Found length specification, i.e.
                            // {num}, so we need to parse it's
                            // value and pass as length to
                            // this.GenerateSubstring(def, len)
                            if (i < pattern.Length &&
                                pattern[i + 1] == '{')
                            {
                                i++;
                                while (true)
                                {
                                    i++;
                                    if (i == pattern.Length ||
                                        pattern[i] == '}') break;
                                    length += pattern[i];
                                }
                            }
                            break;
                        }
                        definition += pattern[i];
                        i++;
                    }

                    // Trying to parse pattern length.
                    if (definition != "")
                    {
                        if (!int.TryParse(length, out len)) len = 1;
                        result += this.GenerateSubstring(definition, len);
                    }
                }
                else if (pattern[i] != '\\') result += pattern[i];
                i++;
            }

            return result;
        }
    }
}
