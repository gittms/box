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
        /// Gets aggregators column.
        /// </summary>
        public Column Column
        {
            get { return this.column; }
        }

        // AUTODOC: Aggregator.Name
        public string Name
        {
            get { return this.column.Name; }
        }

        // AUTODOC: Column.operator ==(Aggregator First, CAlias Second)
        public static CAlias operator ==(Aggregator First, CAlias Second)
        {
            Second.Column = First;
            return Second;
        }

        // AUTODOC: Column.operator !=(Aggregator First, CAlias Second)
        public static CAlias operator !=(Aggregator First, CAlias Second)
        {
            throw new ArgumentException(
                "Unable to compare Aggregator to CAlias object.");
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
