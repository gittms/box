using System;

namespace Definitif.Data.ObjectSql.Aggregator
{
    /// <summary>
    /// Represents MIN aggregator
    /// MIN( [IColumn] )
    /// </summary>
    public class Min : Aggregator
    {
        public Min(Column Column)
        {
            this.column = Column;
        }
    }
}
