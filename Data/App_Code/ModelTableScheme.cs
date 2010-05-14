using System;
using System.Collections.Generic;

namespace Definitif.Data
{
    /// <summary>
    /// Represents base class for model table scheme object.
    /// </summary>
    public class ModelTableScheme<MapperType> : IModelTableScheme
        where MapperType : IMapper, new()
    {
        protected static Table table = new MapperType().Table;

        private Column p_Id = table["Id"];
        public Column Id { get { return p_Id; } }

        private Column p_Version = table["Version"];
        public Column Version { get { return p_Version; } }
    }
}
