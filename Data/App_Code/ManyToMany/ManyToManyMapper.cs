using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Definitif.Data;
using Definitif.Data.Queries;

namespace Definitif.Data
{
    /// <summary>
    /// Generic mapper used for retrieving lists of many-to-many relation objects.
    /// </summary>
    /// <typeparam name="L">Link type.</typeparam>
    /// <typeparam name="M">Model type.</typeparam>
    public class ManyToManyMapper<L, M> : Mapper<L>
        where L : class, IManyToMany, new()
        where M : class, IModel, new()
    {
        protected Mapper<L> linkMapper;
        protected Mapper<M> modelMapper;
        protected string joinField;
        protected string whereField;

        public ManyToManyMapper()
        {
            L l = Singleton<L>.Default;
            M m = Singleton<M>.Default;
            this.linkMapper = l.IMapper() as Mapper<L>;
            this.modelMapper = m.IMapper() as Mapper<M>;
            this.joinField = (this.linkMapper as IManyToManyMapper).FieldNameJoin(m);
            this.whereField = (this.linkMapper as IManyToManyMapper).FieldNameWhere(m);
        }

        /// <summary>
        /// Gets list of many-to-many relation objects for specified model.
        /// </summary>
        /// <param name="id">Id of model to get relations for.</param>
        /// <returns>List of ManyToMany objects.</returns>
        public List<ManyToMany<L, M>> Get(Id id)
        {
            return Get(Singleton<M>.Default.C.Id == id.Value);
        }

        /// <summary>
        /// Gets list of many-to-many relation objects.
        /// </summary>
        /// <param name="expression">Expression for building the SQL query.</param>
        /// <returns>List of ManyToMany objects.</returns>
        public List<ManyToMany<L, M>> Get(Expression expression)
        {
            List<ManyToMany<L, M>> result = new List<ManyToMany<L, M>>();
            IDataReader reader = ExecuteReader(ReadCommand(expression));
            string prefix = this.linkMapper.Table.Name + ".";
            while (reader.Read())
            {
                ManyToMany<L, M> item = new ManyToMany<L, M>();
                item.Link = this.linkMapper.ReadObject(reader, prefix);
                item.Model = this.modelMapper.ReadObject(reader);
                result.Add(item);
            }
            reader.Close();
            return result;
        }

        /// <summary>
        /// Returns the query for retrieving ManyToMany objects.
        /// </summary>
        /// <param name="expression">Expression for building the SQL query.</param>
        /// <returns>DbCommand instance.</returns>
        protected virtual DbCommand ReadCommand(Expression expression)
        {
            Query query = new Select<M>()
                .InnerJoin<L>((m, l) => m.C.Id == l.C[joinField])
                .Fields((m, l) => l.C["*"] & m.C["**"])
                .Where(expression);
            return this.database.GetCommand(query);
        }

        public override L ReadObject(IDataReader reader)
        {
            throw new NotImplementedException();
        }

        protected override List<DbCommand> UpdateCommands(L obj)
        {
            throw new NotImplementedException();
        }

        protected override List<DbCommand> InsertCommands(L obj)
        {
            throw new NotImplementedException();
        }

        protected override List<DbCommand> DeleteCommands(L obj)
        {
            throw new NotImplementedException();
        }
    }
}
