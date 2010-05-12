using System;

namespace Definitif.Data
{
    /// <summary>
    /// Container with a model and its parametrized link to another model.
    /// Used for representing many-to-many relations between models.
    /// </summary>
    /// <typeparam name="L">Link type.</typeparam>
    /// <typeparam name="M">Model type.</typeparam>
    public class ManyToMany<L, M> : Model<ManyToManyMapper<L, M>>
        where L : class, IManyToMany, new()
        where M : class, IModel, new()
    {
        protected L link = new L();
        protected M model = new M();

        /// <summary>
        /// Gets or sets the link object.
        /// </summary>
        public L Link
        {
            get { return this.link; }
            set { this.link = value; }
        }

        /// <summary>
        /// Gets or sets the model object.
        /// </summary>
        public M Model
        {
            get { return this.model; }
            set { this.model = value; }
        }
    }
}
