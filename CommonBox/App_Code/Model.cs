using System;
using System.Collections.Generic;

namespace Definitif.Data.CommonBox
{
    /// <summary>
    /// Base entity class.
    /// </summary>
    /// <typeparam name="M">Specific IMapper type to be associated with this entity.</typeparam>
    public abstract class Model<M> : IModel
        where M : IMapper, new()
    {
        protected Id id = Id.Empty;
        protected int version = 1;

        protected M mapper = new M();
        protected List<Id> subscribed = new List<Id>();

        /// <summary>s
        /// Gets or sets the Id.
        /// </summary>
        public Id Id
        {
            get { return id; }
            set
            {
                this.id = value;
                foreach (Id i in subscribed) i.SetValue(value.Value);
                this.subscribed = new List<Id>();
            }
        }

        /// <summary>
        /// Gets or sets object version.
        /// This is used for checking when writing to database.
        /// </summary>
        public int Version
        {
            get { return this.version; }
            set { this.version = value; }
        }

        /// <summary>
        /// Creates empty instance.
        /// </summary>
        protected Model()
        {
            this.id = Id.Empty;
            this.version = 1;
        }

        /// <summary>
        /// When overridden in derived class, returns a cloned copy of this instance.
        /// </summary>
        /// <returns>Cloned Model instance.</returns>
        public abstract Model<M> Clone();

        /// <summary>
        /// Stores this object.
        /// </summary>
        public void Save()
        {
            this.mapper.Write(this);
        }

        /// <summary>
        /// Deletes this object.
        /// </summary>
        public void Delete()
        {
            this.mapper.Delete(this);
        }

        /// <summary>
        /// Reads the object with specified Id.
        /// </summary>
        /// <param name="id">Object Id.</param>
        public void Read(Id id)
        {
            this.mapper.ReadInto(this, id);
        }

        /// <summary>
        /// Gets the Mapper associated with this Model.
        /// </summary>
        /// <returns>Mapper instance.</returns>
        public IMapper Mapper
        {
            get { return this.mapper; }
        }

        /// <summary>
        /// Subscribes specified Id to be updated when this object's Id changes.
        /// </summary>
        /// <param name="id">Id instance to be subscribed.</param>
        public void SubscribeId(Id id)
        {
            this.subscribed.Add(id);
        }
    }
}
