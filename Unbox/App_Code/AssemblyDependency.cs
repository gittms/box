using System;
using System.Xml;
using System.Xml.Serialization;

namespace Definitif.Box.Unbox
{
    [XmlRoot(ElementName = "dependency")]
    public class AssemblyDependency
    {
        /// <summary>
        /// Gets dependency assembly name.
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name;

        /// <summary>
        /// Gets dependency assembly repository.
        /// </summary>
        [XmlAttribute(AttributeName = "repo")]
        public string Repository;

        /// <summary>
        /// Gets dependency assembly min version.
        /// </summary>
        [XmlAttribute(AttributeName = "minVersion")]
        public string Version;
    }
}
