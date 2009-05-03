using System;
using System.Collections;
using System.Collections.Generic;

namespace Definitif.Data.ObjectSql
{
    /// <summary>
    /// Represents table column object.
    /// </summary>
    public class Column : Operators, IColumn
    {
        private string name;
        private ITable table;

        /// <summary>
        /// Gets column name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }
        /// <summary>
        /// Gets or sets parent Table object.
        /// </summary>
        internal ITable Table
        {
            get { return this.table; }
            set { this.table = value; }
        }

        public Column(
            string Name)
        {
            this.name = Name;
        }

        // AUTODOC: Column.SUM(Column Column)
        public static Aggregator.Aggregator SUM(Column Column)
        {
            return new 
                Aggregator.Sum(Column);
        }

        // AUTODOC: Column.MAX(Column Column)
        public static Aggregator.Aggregator MAX(Column Column)
        {
            return new
                Aggregator.Max(Column);
        }

        // AUTODOC: Column.MIN(Column Column)
        public static Aggregator.Aggregator MIN(Column Column)
        {
            return new
                Aggregator.Min(Column);
        }

        // AUTODOC: Column.Alias(string Name)
        public static CAlias Alias(string Name)
        {
            return new
                CAlias(Name);
        }

        // AUTODOC: Column.operator ==(Column First, CAlias Second)
        public static CAlias operator ==(Column First, CAlias Second)
        {
            Second.Column = First;
            return Second;
        }

        // AUTODOC: Column.operator !=(Column First, CAlias Second)
        public static CAlias operator !=(Column First, CAlias Second)
        {
            throw new ArgumentException(
                "Unable to compare Column to CAlias object.");
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
