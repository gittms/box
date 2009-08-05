using System;

namespace Definitif.Data.CommonBox
{
    /// <summary>
    /// An Id field of database-stored objects.
    /// </summary>
    public class Id
    {
        private object value = null;

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
            this.value = value;
        }

        /// <summary>
        /// Gets an empty Id.
        /// </summary>
        public static Id Empty
        {
            get { return new Id(); }
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
            this.value = value;
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
            if ((a as object) == null || (b as object) == null)
            {
                return false;
            }
            return a.value == b.value;
        }

        public static bool operator !=(Id a, Id b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is Id)
            {
                return this.value == (obj as Id).value;
            }
            else return false;
        }

        public override int GetHashCode()
        {
            if (this.value == null) return 0;
            else return this.value.GetHashCode();
        }
    }
}
