using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Definitif.VisualStudio.Generator
{
    /// <summary>
    /// Represents .box code document object model.
    /// </summary>
    public class CodeDom
    {
        public List<Namespace> Namespaces { get; set; }

        /// <summary>
        /// Parses passed input string and returns CodeDom instance.
        /// </summary>
        /// <param name="input">.box file contents.</param>
        /// <returns>CodeDom instance.</returns>
        [Obsolete("Parsing string is obsolete as it does not support inheritance processing. " +
                  "Use ParseFile() instead.")]
        public static CodeDom Parse(string input)
        {
            CodeDom dom = new CodeDom();
            CodeParser.Parse(dom, input);

            return dom;
        }

        private static CodeDom ParseFileWithoutInheritance(string filename)
        {
            CodeDom dom = new CodeDom();
            CodeParser.Parse(dom, File.ReadAllText(filename));

            return dom;
        }

        /// <summary>
        /// Parses passed file and return CodeDom instance.
        /// </summary>
        /// <param name="filename">.box file path.</param>
        /// <returns>CodeDom instance.</returns>
        public static CodeDom ParseFile(string filename, string rootPath = null)
        {
            CodeDom dom = ParseFileWithoutInheritance(filename);

            if (rootPath == null) rootPath = filename;
            dom.ProcessInheritance(rootPath);

            return dom;
        }

        /// <summary>
        /// Processes inheritance for current CodeDom instance.
        /// </summary>
        /// <param name="rootPath">Project root path.</param>
        private void ProcessInheritance(string rootPath)
        {
            List<string> modelsInFile = GetModelNames(this);

            foreach (Namespace ns in this.Namespaces)
            {
                foreach (Model model in ns.Models)
                {
                    if (model.InheritedModels == null) continue;
                    List<Model> inherritedModels = new List<Model>();

                    while (model.InheritedModels.Count > 0)
                    {
                        // Searching for model declaration.
                        Model orig = model.InheritedModels[0];
                        Model found = CodeDom.FindModel(rootPath, orig.FullName(ns));

                        if (found == null)
                        {
                            throw new FormatException("Unable to find model '" + orig.FullName(ns) + "'.");
                        }

                        model.InheritedModels.Remove(orig);
                        inherritedModels.Add(found);

                        // Adding models new one is inherited from to queue.
                        if (found.InheritedModels == null) continue;
                        foreach (Model inh in found.InheritedModels)
                        {
                            // TODO: Model names should be compared with namespaces.
                            if (inh.Name == model.Name)
                            {
                                throw new FormatException("Circular inheritance in model '" + model.FullName(ns) + "'.");
                            }

                            // TODO: Model names should be compared with namespaces.
                            if (model.InheritedModels.Count(m => m.Name == inh.Name) == 0)
                            {
                                model.InheritedModels.Add(inh);
                            }
                        }
                    }

                    model.InheritedModels = inherritedModels;

                    // Expanding model with members from inherited models.
                    foreach (Model inh in model.InheritedModels)
                    {
                        model.Extend(inh);
                    }
                }
            }
        }

        /// <summary>
        /// Gets list of full model names in file.
        /// </summary>
        private static List<string> GetModelNames(CodeDom dom)
        {
            List<string> modelsInFile = new List<string>();

            foreach (Namespace ns in dom.Namespaces)
            {
                foreach (Model model in ns.Models)
                {
                    modelsInFile.Add(model.FullName(ns));
                }
            }

            return modelsInFile;
        }

        private static Model FindModel(string rootPath, string name)
        {
            // If given path is a file name, getting
            // it's parent directory as a root.
            if (File.Exists(rootPath)) rootPath = new FileInfo(rootPath).DirectoryName;

            // Iterating through all model definitions
            // and searching for model required.
            foreach (FileInfo file in new DirectoryInfo(rootPath).EnumerateFiles("*.box", SearchOption.AllDirectories))
            {
                CodeDom dom = CodeDom.ParseFileWithoutInheritance(file.FullName);

                foreach (Namespace ns in dom.Namespaces)
                {
                    foreach (Model model in ns.Models)
                    {
                        if (model.FullName(ns) == name) return model;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Generates source code for given CodeDom.
        /// </summary>
        /// <param name="defaultNamespace">Default namespace name.</param>
        /// <returns>String containing source code.</returns>
        public string Generate(string defaultNamespace)
        {
            return CodeGenerator.Generate(this, defaultNamespace);
        }
    }
}
