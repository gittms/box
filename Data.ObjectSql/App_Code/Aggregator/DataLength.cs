using System;

namespace Definitif.Data.ObjectSql.Aggregator
{
    /// <summary>
    /// Represents DATALENGTH aggregator
    /// DATALENGTH( [IColumn] )
    /// </summary>
    public class DataLength : Aggregator
    {
        public DataLength(Column Column)
        {
            this.column = Column;
        }
    }
}
