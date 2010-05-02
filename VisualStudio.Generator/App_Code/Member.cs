using System;

namespace Definitif.VisualStudio.Generator
{
    /// <summary>
    /// Represents model member object.
    /// </summary>
    public class Member
    {
        /// <summary>
        /// Gets or sets model autodoc.
        /// </summary>
        public string Autodoc { get; set; }
        /// <summary>
        /// Gets or sets member name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets member type.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets column name.
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// Gets or sets column casting type.
        /// </summary>
        public string ColumnCastingType { get; set; }
        /// <summary>
        /// Gets or sets member modifiers.
        /// </summary>
        public Modifier Modifiers { get; set; }

        /// <summary>
        /// Gets member protected name.
        /// </summary>
        public string NameProtected
        {
            get { return "p_" + this.Name.ToLower(); }
        }

        /// <summary>
        /// Gets or sets member body.
        /// </summary>
        public string MemberBody { get; set; }

        public Member()
        {
            this.Modifiers = Modifier.Default;
        }
    }
}
