using System;
using System.IO;
using System.Collections.Generic;

namespace Definitif.VisualStudio.Generator
{
    /// <summary>
    /// Represents .box code document object model.
    /// </summary>
    public class CodeDom
    {
        public List<Namespace> Namespaces { get; set; }

        /// <summary>
        /// Parses passed input string and returns CodeDom instance.
        /// </summary>
        /// <param name="input">.box file contents.</param>
        /// <returns>CodeDom instance.</returns>
        public static CodeDom Parse(string input)
        {
            CodeDom dom = new CodeDom();
            CodeParser.Parse(dom, input);
            return dom;
        }

        /// <summary>
        /// Parses passed file and return CodeDom instance.
        /// </summary>
        /// <param name="filename">.box file path.</param>
        /// <returns>CodeDom instance.</returns>
        public static CodeDom ParseFile(string filename)
        {
            return CodeDom.Parse(File.ReadAllText(filename));
        }

        /// <summary>
        /// Generates source code for given CodeDom.
        /// </summary>
        /// <returns>String containing source code.</returns>
        public string Generate()
        {
            return CodeGenerator.Generate(this);
        }
    }
}
