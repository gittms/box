using System;
using System.Collections.Generic;
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
            string lib = "lib"; bool gac = false;
            string[] extra;

            // Parsing command line options.
            OptionSet options = new OptionSet()
            {
                { "install", "installs selected assembly with" + nl +
                             "it's dependencies;",
                    v => install = (v != null) },
                { "gac", "installs assemblies to GAC;",
                    v => gac = (v != null) },
                { "lib=", "directory to copy assemblies to" + nl +
                          "(default: lib);" + nl,
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
                W("ERROR: " + e.Message);

                return;
            }

            /* 
             * Showing usage and options help.
             */
            if (help || (extra.Length == 0 && !list) || 
                !(install || search || list))
            {
                W("Usage: unbox [OPTIONS] assemblies");
                W("Installs specified assemblies to local system." + nl);
                options.WriteOptionDescriptions(Console.Out);
                W("More info on: http://unbox.definitif.ru/.");

                return;
            }

            // Reading repository information.
            Repository repository = new Repository(repo, forceReload: reload);

            /*
             * Listing assemblies available in current repository.
             */
            if (list)
            {
                W("Repository '{0}' contains: " + nl, repo);
                W("    " +
                    String.Join(nl + "    ", repository.Assemblies) + nl + nl +
                    "You can now install selected assembly using:" + nl +
                    "    unbox --install [name]");
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

                W(result.Count > 0 ?
                    "    " + String.Join(nl + "    ", result) + nl + nl +
                        "You can now install selected assembly using:" + nl +
                        "    unbox --install [name]" :
                    "Nothing found...");

                return;
            }

            /*
             * Installing given assemblies.
             */
            else if (install)
            {
                // Validating provided assemblies list.
                foreach (string assembly in extra)
                {
                    if (repository.Contains(assembly)) continue;

                    W("Unknown assembly: " + assembly);
                    return;
                }
            }
        }

        static string nl = Environment.NewLine;
        private static void W(string line)
        {
            Console.WriteLine(line);
        }
        private static void W(string line, params object[] args)
        {
            Console.WriteLine(line, args);
        }
    }
}
