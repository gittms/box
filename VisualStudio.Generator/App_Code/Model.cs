using System;
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
    }
}