using System;
using NDesk.Options;

namespace Definitif.Box.Unbox
{
    partial class Program
    {
        static void Main(string[] args)
        {
            bool install = false, search = false, list = false, help = false;
            string repo = "unbox.definitif.ru"; bool reload = false;
            string lib = "Bin"; bool gac = false; bool silent = false;
            string[] extra;

            // Parsing command line arguments.
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

            // Showing help if no mandatory arguments
            // specified, or --help argument passed.
            if (help || (extra.Length == 0 && !list) || 
                !(install || search || list))
            {
                W("Usage: unbox [OPTIONS] assemblies" + nl);
                options.WriteOptionDescriptions(Console.Out);
                W("More info on: http://unbox.definitif.ru/.");

                return;
            }

            // Validating permissions.
            if (gac && !System.IsAdministrator)
            {
                WC("You should be an Administrator to install assemblies to GAC.", ConsoleColor.Red);
                return;
            }

            // Reading repository information.
            Repository repository = new Repository(repo, forceReload: reload);

            // Handling common commands.
            if (list)
            {
                List(repository);
                return;
            }
            else if (search)
            {
                Search(repository, extra);
                return;
            }
            else if (install)
            {
                Install(repository, extra, lib, silent, gac);
            }
        }
    }
}
