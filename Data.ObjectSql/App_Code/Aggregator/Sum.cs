using System;

namespace Definitif.Data.ObjectSql.Aggregator
{
    /// <summary>
    /// Represents SUM aggregator
    /// SUM( [IColumn] )
    /// </summary>
    public class Sum : Aggregator
    {
        public Sum(Column Column)
        {
            this.column = Column;
        }
    }
}
