using System;

namespace Definitif.Box.Unbox
{
    partial class Program
    {
        static Action youCanNowInstall = () => {
            W(nl + "You can now install selected assembly using:" + nl +
                "    unbox --install [name]");
        };

        /// <summary>
        /// Lists assemblies available in repository.
        /// </summary>
        static void List(Repository repository)
        {
            W("Repository '{0}' contains: " + nl, repository.Name);
            WIndent();
            W(String.Join(nl, repository.Assemblies));
            WUnindent();
            youCanNowInstall();
        }
    }
}
