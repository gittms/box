using System;
using System.Collections.Generic;
using System.Linq;

namespace Definitif.Box.Unbox
{
    partial class Program
    {
        /// <summary>
        /// Searches repository for given strings.
        /// </summary>
        static void Search(Repository repository, IEnumerable<string> strings)
        {
            W("Searching for: " + String.Join(" OR ", strings) + "..." + nl);
            List<string> result = new List<string>();
            foreach (string assembly in strings)
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
        }
    }
}
