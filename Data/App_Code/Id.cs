using System;
using System.Runtime.Serialization;

namespace Definitif.Data
{
    /// <summary>
    /// An Id field of database-stored objects.
    /// </summary>
    public class Id
    {
        private object value = null;
        private static readonly Id empty = new Id();

        /// <summary>
        /// Empty constructor is only used internally to instantiate Id.Empty.
        /// </summary>
        private Id()
        { }

        /// <summary>
        /// Creates new Id with value.
        /// </summary>
        /// <param name="value">Value to initialize instance with.</param>
        public Id(object value)
        {
            this.SetValue(value);
        }

        /// <summary>
        /// Gets an empty Id.
        /// </summary>
        public static Id Empty
        {
            get { return empty; }
        }

        /// <summary>
        /// Gets the Id value.
        /// </summary>
        public object Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Sets current instance's value.
        /// Used internally in subscription.
        /// </summary>
        /// <param name="value">Value to set.</param>
        internal void SetValue(object value)
        {
            if (value is Int32 || value is decimal || value is Int16) this.value = Convert.ToInt64(value);
            else this.value = value;
        }

        /// <summary>
        /// Subscribes this instance to be updated when the Id of specified IModel object changes.
        /// </summary>
        /// <param name="obj">IModel object to subscribe to.</param>
        public void Subscribe(IModel obj)
        {
            obj.SubscribeId(this);
        }

        /// <summary>
        /// Converts this instance to its string representation.
        /// </summary>
        /// <returns>This instance's string representation.</returns>
        public override string ToString()
        {
            if (this.value == null) return "";
            else return this.value.ToString();
        }

        public static bool operator ==(Id a, Id b)
        {
            if ((a as object) != null) return a.Equals(b);
            else if ((b as object) != null) return b.Equals(a);
            else return true;
        }

        public static bool operator !=(Id a, Id b)
        {
            return !(a == b);
        }

        public bool Equals(Id obj)
        {
            if ((obj as object) != null)
            {
                if (this.value == null && obj.value == null) return true;
                else if (this.value == null || obj.value == null) return false;
                else return this.value.Equals((obj as Id).value);
            }
            else if (this.value == null) return true;
            else return false;
        }
        public override bool Equals(object obj)
        {
            if (obj is Id) return this.Equals(obj as Id);
            else if (obj == null && this.value == null) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            if (this.value == null) return 0;
            else return this.value.GetHashCode();
        }
    }
}
