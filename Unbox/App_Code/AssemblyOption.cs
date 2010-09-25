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

        public Assembly assembly;

        public override int GetHashCode()
        {
            return (this.PublicKey + this.Version).GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is AssemblyOption)) return false;
            AssemblyOption option = (AssemblyOption)obj;

            return (option.Version == this.Version &&
                option.PublicKey == this.PublicKey);
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", this.assembly.Name, this.Version);
        }
    }
}
