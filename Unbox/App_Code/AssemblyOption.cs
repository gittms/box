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
        /// Gets option file name.
        /// </summary>
        [XmlAttribute(AttributeName = "fileName")]
        public string FileName;

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

        /// <summary>
        /// Checks if assembly is a bundle.
        /// </summary>
        public bool IsBundle
        {
            get
            {
                return this.GetFileName().EndsWith(".zip");
            }
        }

        public override int GetHashCode()
        {
            return (this.assembly.Name + this.PublicKey + this.Version).GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is AssemblyOption)) return false;
            AssemblyOption option = (AssemblyOption)obj;

            return (option.assembly.Name == this.assembly.Name &&
                option.Version == this.Version &&
                option.PublicKey == this.PublicKey);
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})",
                this.assembly.Name,
                this.Version);
        }

        /// <summary>
        /// Gets assembly option location.
        /// </summary>
        public string Url
        {
            get
            {
                return String.Format("{0}{1}/{2}/{3}",
                    this.assembly.repository.url,
                    this.assembly.Path,
                    this.Version,
                    this.GetFileName());
            }
        }

        public string GetFileName()
        {
            return this.FileName ?? this.assembly.Name + ".dll";
        }
    }
}
