using System;
using System.Linq;
using System.Collections.Generic;

namespace Definitif.VisualStudio.Generator
{
    /// <summary>
    /// Represents model object.
    /// </summary>
    public class Model
    {
        /// <summary>
        /// Gets or sets model autodoc.
        /// </summary>
        public string Autodoc { get; set; }
        /// <summary>
        /// Gets or sets model name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets table name.
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Gets or sets databse reference.
        /// </summary>
        public string DatabaseRef { get; set; }
        /// <summary>
        /// Gets or sets models to inherit from.
        /// </summary>
        public List<Model> InheritedModels { get; set; }
        /// <summary>
        /// Gets or sets model members.
        /// </summary>
        public List<Member> Members { get; set; }
        /// <summary>
        /// Gets or sets model modifiers.
        /// </summary>
        public Modifier Modifiers { get; set; }

        public Model()
        {
            this.Members = new List<Member>();
            this.Modifiers = Modifier.Default;
        }

        /// <summary>
        /// Extends current model with members from given.
        /// </summary>
        public void Extend(Model model)
        {
            foreach (Member member in model.Members)
            {
                // If model does not contain member with same name,
                // adding it to current model.
                if (this.Members.Count(m => m.Name == member.Name) == 0)
                {
                    this.Members.Add(member);
                }
            }
        }

        /// <summary>
        /// Generates full name for model in a given namespace.
        /// </summary>
        internal string FullName(Namespace ns)
        {
            if (String.IsNullOrEmpty(ns.Name))
            {
                return this.Name;
            }

            return ns.Name + "." + this.Name;
        }
    }
}