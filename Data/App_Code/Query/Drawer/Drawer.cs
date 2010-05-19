using System;

namespace Definitif.Data.Queries
{
    /// <summary>
    /// Represents abstract query drawer.
    /// </summary>
    public abstract class Drawer
    {
        public abstract string Identity { get; }

        public virtual string Draw(Query query)
        {
            throw new NotImplementedException();
        }
    }
}
