using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Definitif.Box.Unbox
{
    /// <summary>
    /// Represents Unbox repository.
    /// </summary>
    public class Repository
    {
        internal string url;
        public string Name;
        private XmlDocument repo = new XmlDocument();

        /// <summary>
        /// Gets location of repository cache directory.
        /// </summary>
        private static string GetLocalCacheDirectory()
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            directory = Path.Combine(directory, "Definitif", "Definitif Unbox");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            return directory;
        }

        /// <summary>
        /// Gets location of repository cache.
        /// </summary>
        public static string GetCacheLocation(string hostname)
        {
            return Path.Combine(GetLocalCacheDirectory(), hostname);
        }

        /// <summary>
        /// Checks if repository is cached and cache is not older than an hour.
        /// </summary>
        public static bool IsCached(string hostname)
        {
            string cache = GetCacheLocation(hostname);

            return (File.Exists(cache) &&
                DateTime.Now - new FileInfo(cache).LastWriteTime < new TimeSpan(1, 0, 0));
        }

        /// <summary>
        /// Reads repository contents from hostname specified.
        /// </summary>
        public Repository(string hostname, bool forceReload = false)
        {
            // Formatting hostname to url and loading it.
            url = String.Format("http://{0}/repo/", hostname);
            this.Name = hostname;

            string cache = GetCacheLocation(hostname);
            if (forceReload || !IsCached(hostname))
            {
                XmlReader reader = new XmlTextReader(url);
                XmlWriter writer = new XmlTextWriter(cache, Encoding.UTF8);

                repo.Load(reader);
                repo.WriteTo(writer);

                reader.Close();
                writer.Close();
            }
            else
            {
                repo.Load(cache);
            }
        }

        /// <summary>
        /// Gets assemblies string list.
        /// </summary>
        public string[] Assemblies
        {
            get
            {
                if (assemblies == null)
                {
                    XmlNodeList nodes = repo.SelectNodes("//repository/assemblies/assembly");
                    assemblies = new string[nodes.Count];
                    for (int i = 0; i < nodes.Count; i++) assemblies[i] = nodes[i].Attributes["name"].InnerText;
                }
                return assemblies;
            }
        }
        private string[] assemblies;

        /// <summary>
        /// Performs case-insensitive search over repository assemblies names.
        /// </summary>
        /// <param name="exact">If True, exact search will be performed.</param>
        public string[] Search(string name, bool exact = false)
        {
            // Making name lowercase for case-insensitive search.
            name = name.ToLower();

            return this.Assemblies
                .Where<string>(n => exact ? n.ToLower() == name : n.ToLower().Contains(name))
                .ToArray();
        }

        /// <summary>
        /// Checks if repository contains assembly specified.
        /// </summary>
        public bool Contains(string name)
        {
            return this.Search(name, exact: true).Length != 0;
        }

        /// <summary>
        /// Gets assembly from repository by name.
        /// </summary>
        public Assembly Get(string name)
        {
            // Making name lowercase for case-insensitive search.
            name = name.ToLower();

            // Checking if repository contains requested assembly.
            if (!this.Contains(name))
                throw new Exception("Repository does not contain this assembly.");

            // Getting assembly instance by name.
            string path = repo.SelectSingleNode("//repository/assemblies/assembly" + 
                "[translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='" + name + "']")
                .Attributes["path"].InnerText;
            XmlReader reader = new XmlTextReader(url + path);

            XmlSerializer serializer = new XmlSerializer(typeof(Assembly));
            Assembly assembly = (Assembly)serializer.Deserialize(reader);

            // Setting references between objects.
            assembly.Path = path;
            if (assembly.Options == null) assembly.Options = new AssemblyOption[0];
            foreach (AssemblyOption option in assembly.Options)
            {
                option.assembly = assembly;
                if (option.Dependencies == null) option.Dependencies = new AssemblyDependency[0];
                foreach (AssemblyDependency dependency in option.Dependencies)
                {
                    dependency.option = option;
                }
            }
            assembly.repository = this;

            reader.Close();
            return assembly;
        }
    }
}
