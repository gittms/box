using System;

namespace Definitif.Data
{
    /// <summary>
    /// Represents base class for model table scheme object.
    /// </summary>
    public class ModelTableScheme<ModelType, MapperType> : IModelTableScheme
        where ModelType : IModel
        where MapperType : Mapper<ModelType>, new()
    {
        protected static Table table = new MapperType().Table;

        public Column Id
        {
            get { return table["Id"]; }
        }

        public Column Version
        {
            get { return table["Version"]; }
        }
    }
}
