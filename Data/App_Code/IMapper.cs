using System;
using System.Data;
using System.Data.Common;

namespace Definitif.Data
{
    /// <summary>
    /// Partially defines an object-relational mapper.
    /// This is a helper interface, not all the methods are defined.
    /// </summary>
    public interface IMapper
    {
        void Write(IModel obj);
        void Write(DbConnection con, IModel obj);
        void Delete(IModel obj);
        void Delete(DbConnection con, IModel obj);

        DbConnection GetConnection();

        Table Table { get; }
    }
}
