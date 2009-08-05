using System;
using System.Data;

namespace Definitif.Data.CommonBox
{
    /// <summary>
    /// Partially defines an object-relational mapper.
    /// This is a helper interface, not all the methods are defined.
    /// </summary>
    public interface IMapper
    {
        void ReadInto(IModel obj, Id id);
        void Write(IModel obj);
        void Write(IDbConnection con, IDbTransaction trans, IModel obj);
        void Delete(IModel obj);
        void Delete(IDbConnection con, IDbTransaction trans, IModel obj);

        IDbConnection GetConnection();
    }
}
