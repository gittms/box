using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Definitif.Box.Unbox
{
    partial class Program
    {
        /// <summary>
        /// Installs given assemblies.
        /// </summary>
        static void Install(Repository repository, IEnumerable<string> strings, string lib, bool silent, bool gac)
        {
            // Building dependencies list.
            W("Building assemblies list...");
            List<AssemblyOption> assemblies = new List<AssemblyOption>();

            // Validating provided assemblies list.
            foreach (string assembly in strings)
            {
                if (repository.Contains(assembly))
                {
                    // Calculating dependency trees.
                    assemblies.AddRange(repository.Get(assembly)
                        .GetAssemblyOptionWithDependencies());
                    continue;
                }

                W("Unknown assembly: " + assembly);
                return;
            }

            // Showing assemblies list with versions.
            AssemblyOption[] toInstall = assemblies.Distinct().ToArray();
            W("Following assemblies will be installed:");
            WIndent();
            W(nl + String.Join(nl, toInstall.Select<AssemblyOption, string>(o => o.ToString())) + nl);
            WUnindent();

            // Confirming action.
            while (true)
            {
                if (silent) break;
                Console.Write("Continue? [y/n]:");
                ConsoleKeyInfo key = Console.ReadKey();
                Console.WriteLine();
                if (key.KeyChar == 'n') return;
                if (key.KeyChar == 'y') break;
            }

            // Performing assemblies installation.
            WebClient client = new WebClient();
            W("Performing assemblies installation:" + nl);
            WIndent();

            if (!gac)
            {
                // Installing to Bin (or other overrided) directory.
                if (!Directory.Exists(lib)) Directory.CreateDirectory(lib);

                foreach (AssemblyOption assembly in toInstall)
                {
                    string path = Path.Combine(lib, assembly.GetFileName());
                    W("Downloading {0} to: {1}...", assembly.assembly.Name, path);
                    client.DownloadFile(assembly.Url, path);
                }
            }
            else
            {
                // Installing to Global Assembly Cache.
                string temp = Path.GetTempPath();

                foreach (AssemblyOption assembly in toInstall)
                {
                    string path = Path.Combine(temp, assembly.GetFileName());
                    W("Installing {0}...", assembly.ToString());
                    client.DownloadFile(assembly.Url, path);
                    System.InstallAssemblyToGac(path);
                }
            }

            W(""); WUnindent();
            WC("Installation complete!", ConsoleColor.DarkGreen);
        }
    }
}
