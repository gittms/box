using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NDesk.Options;

namespace Definitif.Box.Unbox
{
    class Program
    {
        static string nl = Environment.NewLine;

        static void Main(string[] args)
        {
            bool install = false, search = false, list = false, help = false;
            string repo = "unbox.definitif.ru"; bool reload = false;

            //args = new string[] { "--search", "mysql" };

            // Parsing command line options.
            OptionSet options = new OptionSet()
            {
                { "install", "installs selected assembly with" + nl +
                             "it's dependencies.",
                    v => install = (v != null) },
                { "search", "performs search by given name.",
                    v => search = (v != null) },
                { "list", "lists all available assemblies in repo.",
                    v => list = (v != null) },
                { "help", "shows this message and exits.",
                    v => help = (v != null) },

                { "repo=", "repository to use.",
                    v => { if (v != null) repo = v; } },
                { "nocache", "disables local repo cache for this" + nl +
                             "request.",
                    v => reload = (v != null) },
            };
            string[] extra = options.Parse(args).ToArray();

            // Showing help.
            if (help || (extra.Length == 0 && !list) || 
                !(install || search || list))
            {
                Console.WriteLine("Usage: unbox [OPTIONS] assemblies");
                Console.WriteLine("Installs specified assemblies to local system.");
                options.WriteOptionDescriptions(Console.Out);

                return;
            }

            // Reading repository information.
            Repository repository = new Repository(repo, forceReload: reload);
            if (list)
            {
                Console.WriteLine("Repository '{0}' contains: " + nl, repo);
                Console.WriteLine("    " +
                    String.Join(nl + "    ", repository.Assemblies) + nl + nl +
                    "You can now install selected assembly using:" + nl +
                    "    unbox --install [name]");
                return;
            }
            else if (search)
            {
                Console.WriteLine("Searching for: " + String.Join(" OR ", extra) + "..." + nl);
                List<string> result = new List<string>();
                foreach (string assembly in extra)
                {
                    result.AddRange(repository.Search(assembly)
                        .Where(item => !result.Contains(item)));
                }

                Console.WriteLine(result.Count > 0 ?
                    "    " + String.Join(nl + "    ", result) + nl + nl +
                        "You can now install selected assembly using:" + nl +
                        "    unbox --install [name]" :
                    "Nothing found +(");

                return;
            }
            else if (install)
            {
                // Validating provided assemblies list.
                foreach (string assembly in extra)
                {
                    if (repository.Contains(assembly)) continue;

                    Console.WriteLine("Unknown assembly: " + assembly);
                    return;
                }
            }
        }
    }
}
