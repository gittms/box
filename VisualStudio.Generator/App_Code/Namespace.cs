using System;
using System.Collections.Generic;

namespace Definitif.VisualStudio.Generator
{
    /// <summary>
    /// Represents code namespace.
    /// </summary>
    public class Namespace
    {
        public string Name { get; set; }
        public List<Model> Models { get; set; }
        public Namespace Parent { get; set; }

        public Namespace()
        {
            this.Models = new List<Model>();
        }

        /// <summary>
        /// Represents default code namespace.
        /// </summary>
        public class Default : Namespace
        {
            public Default() : base()
            { }
        }
    }
}
