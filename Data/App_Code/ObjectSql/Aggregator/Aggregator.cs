using System;

namespace Definitif.Data.ObjectSql.Aggregator
{
    /// <summary>
    /// Represents ObjectSql column aggregator.
    /// </summary>
    public abstract class Aggregator : Operators, IColumn
    {
        protected Column column;

        /// <summary>
        /// Gets aggregated column.
        /// </summary>
        public Column Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Gets table aggregator applies to.
        /// </summary>
        public ITable Table
        {
            get { return this.column.Table; }
        }

        /// <summary>
        /// Gets aggregated column name.
        /// </summary>
        public string Name
        {
            get { return this.column.Name; }
        }

        public static ColumnAlias operator ==(Aggregator First, ColumnAlias Second)
        {
            Second.Column = First;
            return Second;
        }

        public static ColumnAlias operator !=(Aggregator First, ColumnAlias Second)
        {
            throw new ArgumentException("Unable to compare Aggregator to ColumnAlias object.");
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
