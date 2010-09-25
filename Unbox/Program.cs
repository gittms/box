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

                AssemblyOption[] toInstall = assemblies.Distinct().ToArray();
                W("Following assemblies will be installed:");
                WIndent();
                W(String.Join(nl, toInstall.Select<AssemblyOption, string>(o => o.ToString())));
                WUnindent();
            }
        }

        static string nl = Environment.NewLine;
        static int indent = 0;
        private static void W(string line)
        {
            W(line, "");
        }
        private static void W(string line, params object[] args)
        {
            string indentStr = "".PadLeft(indent, ' ');
            line = indentStr + line.Replace(nl, nl + indentStr);
            Console.WriteLine(line, args);
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
