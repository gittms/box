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
        private string url;
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
                DateTime.Now - new FileInfo(cache).CreationTime < new TimeSpan(1, 0, 0));
        }

        /// <summary>
        /// Reads repository contents from hostname specified.
        /// </summary>
        public Repository(string hostname, bool forceReload = false)
        {
            // Formatting hostname to url and loading it.
            url = String.Format("http://{0}/repo/", hostname);

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
        public string[] Search(string name)
        {
            name = name.ToLower();
            return this.Assemblies
                .Where<string>(n => n.ToLower().Contains(name))
                .ToArray();
        }

        /// <summary>
        /// Gets assembly from repository by name.
        /// </summary>
        public Assembly Get(string name)
        {
            string path = repo.SelectSingleNode("//repository/assemblies/assembly[@name='" + name + "']").Attributes["path"].InnerText;
            XmlReader reader = new XmlTextReader(url + path);

            XmlSerializer serializer = new XmlSerializer(typeof(Assembly));
            Assembly assembly = (Assembly)serializer.Deserialize(reader);

            reader.Close();
            return assembly;
        }
    }
}
