using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using NDesk.Options;

namespace Definitif.Box.Unbox
{
    class Program
    {
        static void Main(string[] args)
        {
            bool install = false, search = false, list = false, help = false;
            string repo = "unbox.definitif.ru"; bool reload = false;
            string lib = "Bin"; bool gac = false; bool silent = false;
            string[] extra;

            /*
             * Parsing command line arguments.
             */
            OptionSet options = new OptionSet()
            {
                { "install", "installs selected assembly with" + nl +
                             "it's dependencies;",
                    v => install = (v != null) },
                { "gac", "installs assemblies to GAC;",
                    v => gac = (v != null) },
                { "silent|quite", "silently installs without confirmations;",
                    v => silent = (v != null) },
                { "lib=", "directory to copy assemblies to" + nl +
                          "(default: Bin);" + nl,
                    v => { if (v != null) lib = v; } },

                { "search", "performs search by given name;",
                    v => search = (v != null) },
                { "list", "lists all available assemblies in repo;",
                    v => list = (v != null) },

                { "repo=", "repository to use;",
                    v => { if (v != null) repo = v; } },
                { "nocache", "disables local repo cache for this" + nl +
                             "request;" + nl,
                    v => reload = (v != null) },

                { "help", "shows this message and exits." + nl,
                    v => help = (v != null) },
            };

            try
            {
                extra = options.Parse(args).ToArray();
            }
            catch (OptionException e)
            {
                WC(e.Message, ConsoleColor.Red);
                return;
            }

            /* 
             * Showing usage and options help.
             */
            if (help || (extra.Length == 0 && !list) || 
                !(install || search || list))
            {
                W("Usage: unbox [OPTIONS] assemblies" + nl);
                options.WriteOptionDescriptions(Console.Out);
                W("More info on: http://unbox.definitif.ru/.");

                return;
            }

            /*
             * Validating permissions.
             */
            if (gac && !System.IsAdministrator)
            {
                WC("You should be an Administrator to install assemblies to GAC.", ConsoleColor.Red);
                return;
            }

            /*
             * Lambda messages helpers.
             */
            Action youCanNowInstall = () => {
                W(nl + "You can now install selected assembly using:" + nl +
                    "    unbox --install [name]");
            };

            // Reading repository information.
            Repository repository = new Repository(repo, forceReload: reload);

            /*
             * Listing assemblies available in current repository.
             */
            if (list)
            {
                W("Repository '{0}' contains: " + nl, repo);
                WIndent();
                W(String.Join(nl, repository.Assemblies));
                WUnindent();
                youCanNowInstall();
                return;
            }

            /*
             * Searching repository assemblies for given substrings.
             */
            else if (search)
            {
                W("Searching for: " + String.Join(" OR ", extra) + "..." + nl);
                List<string> result = new List<string>();
                foreach (string assembly in extra)
                {
                    result.AddRange(repository.Search(assembly)
                        .Where(item => !result.Contains(item)));
                }

                WIndent();
                if (result.Count > 0)
                {
                    W(String.Join(nl, result));
                    WUnindent();
                    youCanNowInstall();
                }
                else
                {
                    WUnindent();
                    W("Nothing found...");
                }

                return;
            }

            /*
             * Installing given assemblies.
             */
            else if (install)
            {
                // Building dependencies list.
                W("Building assemblies list...");
                List<AssemblyOption> assemblies = new List<AssemblyOption>();

                // Validating provided assemblies list.
                foreach (string assembly in extra)
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

                W("");  WUnindent();
                WC("Installation complete!", ConsoleColor.DarkGreen);
            }
        }

        static string nl = Environment.NewLine;
        static ConsoleColor defaultColor = Console.ForegroundColor;
        static int indent = 0;
        private static void W(string line)
        {
            W(line, "");
        }
        private static void WC(string line, ConsoleColor color)
        {
            WC(line, color, "");
        }
        private static void W(string line, params object[] args)
        {
            string indentStr = "".PadLeft(indent, ' ');
            line = indentStr + line.Replace(nl, nl + indentStr);
            Console.WriteLine(line, args);
        }
        private static void WC(string line, ConsoleColor color, params object[] args)
        {
            Console.ForegroundColor = color;
            W(line, args);
            Console.ForegroundColor = defaultColor;
        }
        private static void WIndent()
        {
            indent += 4;
        }
        private static void WUnindent()
        {
            indent -= 4;
            if (indent < 0) indent = 0;
        }
    }
}
