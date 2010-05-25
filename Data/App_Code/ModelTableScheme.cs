using System;
using System.Collections.Generic;
using Definitif.Data.Queries;

namespace Definitif.Data
{
    /// <summary>
    /// Represents base class for model table scheme object.
    /// </summary>
    public class ModelTableScheme<MapperType> : IModelTableScheme
        where MapperType : IMapper, new()
    {
        protected static Table table = Singleton<MapperType>.Default.Table;

        public Column this[string name] { get { return table[name]; } }

        private Column p_Id = table["Id"];
        public Column Id { get { return p_Id; } }

        private Column p_Version = table["Version"];
        public Column Version { get { return p_Version; } }

        public static Expression operator ==(ModelTableScheme<MapperType> modelTableScheme, object value)
        {
            return new Expression()
            {
                Type = ExpressionType.Equals,
                Container = { modelTableScheme.Id, value },
            };
        }
        public static Expression operator !=(ModelTableScheme<MapperType> modelTableScheme, object value)
        {
            return new Expression()
            {
                Type = ExpressionType.NotEquals,
                Container = { modelTableScheme.Id, value },
            };
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
