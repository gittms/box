using System;
using System.Collections.Generic;

namespace Definitif.VisualStudio.Generator
{
    public partial class Member
    {
        public static Dictionary<string, string> ColumnDataTypeMapping
            = new Dictionary<string, string>()
            {
                { "string",     "varchar(255)" },
                // Numeric values.
                { "bool",       "bit" },
                { "long",       "bigint" },
                { "double",     "float" },
                // Xml and other serialized.
                { "xmldocument","xml" },
            };
        public static string ColumnDataTypeId = "int";

        /// <summary>
        /// Gets or sets column data type.
        /// </summary>
        public string ColumnDataType
        {
            get
            {
                // If column is foreign key, then using default type.
                if ((this.Modifiers & Modifier.Foreign_key) != 0)
                {
                    return ColumnDataTypeId;
                }

                // Otherwise, using casting type, type and mapping
                // dictionary.
                string type = (this.ColumnCastingType ?? this.Type ?? "").ToLower();

                if (this.columnDataType != null)
                {
                    return this.columnDataType;
                }
                else if (ColumnDataTypeMapping.ContainsKey(type))
                {
                    return ColumnDataTypeMapping[type];
                }
                else
                {
                    return type;
                }
            }
            set { this.columnDataType = value; }
        }
        private string columnDataType;
    }
}
