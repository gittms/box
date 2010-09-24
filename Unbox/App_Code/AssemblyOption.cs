using System;
using System.Xml;
using System.Xml.Serialization;

namespace Definitif.Box.Unbox
{
    [XmlRoot(ElementName = "option")]
    public class AssemblyOption
    {
        /// <summary>
        /// Gets option version.
        /// </summary>
        [XmlAttribute(AttributeName = "version")]
        public string Version;

        /// <summary>
        /// Gets option public key.
        /// </summary>
        [XmlAttribute(AttributeName = "publicKey")]
        public string PublicKey;

        /// <summary>
        /// Gets option remarks.
        /// </summary>
        [XmlElement(ElementName = "remarks", IsNullable = true)]
        public string Remarks;

        /// <summary>
        /// Gets assembly dependencies collection.
        /// </summary>
        [XmlArray(ElementName = "dependencies")]
        [XmlArrayItem(ElementName = "dependency")]
        public AssemblyDependency[] Dependencies;
    }
}
