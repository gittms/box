using System;
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
    }
}
