using System;

namespace Definitif.Data.ObjectSql.Aggregator
{
    /// <summary>
    /// Represents MAX aggregator
    /// MAX( [IColumn] )
    /// </summary>
    public class Max : Aggregator
    {
        public Max(Column Column)
        {
            this.column = Column;
        }
    }
}
