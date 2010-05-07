using System;

namespace Definitif.Data.ObjectSql.Aggregator
{
    /// <summary>
    /// Represents DISTINCT aggregator
    /// DISTINCT( [IColumn] )
    /// </summary>
    public class Distinct : Aggregator
    {
        public Distinct(Column Column)
        {
            this.column = Column;
        }
    }
}
