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

        public AssemblyOption option;

        /// <summary>
        /// Gets assembly option for this dependency.
        /// </summary>
        public AssemblyOption[] GetAssemblyOptionWithDependencies()
        {
            Repository repo = option.assembly.repository;
            if (this.Repository != null)
            {
                repo = new Repository(this.Repository);
            }

            if (!repo.Contains(this.Name)) return null;
            Assembly assembly = repo.Get(this.Name);

            return assembly.GetAssemblyOptionWithDependencies(this.Version);
        }
    }
}
