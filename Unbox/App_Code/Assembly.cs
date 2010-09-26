using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Definitif.Box.Unbox
{
    [XmlRoot(ElementName = "assembly")]
    public class Assembly
    {
        /// <summary>
        /// Gets assembly name.
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name;

        /// <summary>
        /// Gets assembly path.
        /// </summary>
        public string Path;

        /// <summary>
        /// Gets assembly author.
        /// </summary>
        [XmlElement(ElementName = "author")]
        public string Author;

        /// <summary>
        /// Gets assembly homepage.
        /// </summary>
        [XmlElement(ElementName = "homepage")]
        public string Homepage;

        /// <summary>
        /// Gets assembly description.
        /// </summary>
        [XmlElement(ElementName = "description")]
        public string Description;

        /// <summary>
        /// Gets assembly options collection.
        /// </summary>
        [XmlArray(ElementName = "options")]
        [XmlArrayItem(ElementName = "option")]
        public AssemblyOption[] Options;

        public AssemblyOption[] GetAssemblyOptionWithDependencies(string minVersion = "")
        {
            // Selecting latest version.
            AssemblyOption option = this.Options
                .Where(o => Compare(o.Version, minVersion))
                .OrderByDescending<AssemblyOption, string>(o => o.Version)
                .FirstOrDefault<AssemblyOption>();
            if (option == null) return null;

            // Walking through dependencies.
            AssemblyOption[] result = option.Dependencies
                .SelectMany<AssemblyDependency, AssemblyOption>(dep => dep.GetAssemblyOptionWithDependencies())

            // Adding option of current assembly.
                .Union<AssemblyOption>(new AssemblyOption[] { option })
                .Distinct().ToArray();

            return result;
        }

        internal Repository repository;

        private bool Compare(string first, string second)
        {
            return String.Compare(first, second) >= 0;
        }
    }
}
