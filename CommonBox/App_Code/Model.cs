using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

        protected static M mapper;
        protected List<Id> subscribed = new List<Id>();

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        [DataMember(Name = "Id", IsRequired = false)]
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
        [DataMember(Name = "Version", IsRequired = false)]
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
        /// Stores this object.
        /// </summary>
        public virtual void Save()
        {
            this.IMapper().Write(this);
        }

        /// <summary>
        /// Deletes this object.
        /// </summary>
        public virtual void Delete()
        {
            this.IMapper().Delete(this);
        }

        /// <summary>
        /// Gets new Mapper for this model.
        /// </summary>
        public static M Mapper
        {
            get
            {
                if (mapper == null)
                {
                    mapper = new M();
                }
                return mapper;
            }
        }

        /// <summary>
        /// Gets the IMapper associated with this Model.
        /// </summary>
        /// <returns>Mapper instance.</returns>
        public IMapper IMapper()
        {
            return Model<M>.Mapper;
        }

        /// <summary>
        /// Subscribes specified Id to be updated when this object's Id changes.
        /// </summary>
        /// <param name="id">Id instance to be subscribed.</param>
        public void SubscribeId(Id id)
        {
            this.subscribed.Add(id);
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            // Clearing Id and Version values after
            // deserialization for conflicts preservement.
            this.id = Id.Empty;
            this.version = 1;
        }
    }
}
