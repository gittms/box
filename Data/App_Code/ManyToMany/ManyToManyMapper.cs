using System;
using System.Collections.Generic;
using System.Data;
using Definitif.Data;
using Definitif.Data.ObjectSql;
using Definitif.Data.ObjectSql.Query;

namespace Definitif.Data
{
    /// <summary>
    /// Generic mapper used for retrieving lists of many-to-many relation objects.
    /// </summary>
    /// <typeparam name="L">Link type.</typeparam>
    /// <typeparam name="M">Model type.</typeparam>
    public class ManyToManyMapper<L, M> : Mapper<L>
        where L : class, IManyToManyModel, new()
        where M : class, IModel, new()
    {
        protected Mapper<L> linkMapper;
        protected Mapper<M> modelMapper;
        protected string joinField;
        protected string whereField;

        public ManyToManyMapper()
        {
            L l = new L();
            M m = new M();
            this.linkMapper = l.IManyToManyLinkMapper() as Mapper<L>;
            this.modelMapper = m.IMapper() as Mapper<M>;
            this.joinField = (this.linkMapper as IManyToManyLinkMapper).FieldNameJoin(m);
            this.whereField = (this.linkMapper as IManyToManyLinkMapper).FieldNameWhere(m);
        }

        /// <summary>
        /// Gets list of many-to-many relation objects for specified model.
        /// </summary>
        /// <param name="id">Id of model to get relations for.</param>
        /// <returns>List of ManyToMany objects.</returns>
        public List<ManyToManyModel<L, M>> Get(Id id)
        {
            return Get(this.linkMapper.Table[this.whereField] == id.Value);
        }

        /// <summary>
        /// Gets list of many-to-many relation objects.
        /// </summary>
        /// <param name="field">Field to filter by.</param>
        /// <param name="value">Field value.</param>
        /// <returns>List of ManyToMany objects.</returns>
        public List<ManyToManyModel<L, M>> Get(string field, object value)
        {
            return this.Get(this.linkMapper.Table[field] == value);
        }

        /// <summary>
        /// Gets list of many-to-many relation objects.
        /// </summary>
        /// <param name="parameters">Parameters used for building the SQL query.</param>
        /// <returns>List of ManyToMany objects.</returns>
        public List<ManyToManyModel<L, M>> Get(params IExpression[] parameters)
        {
            List<ManyToManyModel<L, M>> result = new List<ManyToManyModel<L, M>>();
            IDataReader reader = ExecuteReader(ReadCommand(parameters));
            string prefix = this.linkMapper.Table.Name + ".";
            while (reader.Read())
            {
                ManyToManyModel<L, M> item = new ManyToManyModel<L, M>();
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
        /// <param name="parameters">Parameters used for building the SQL query.</param>
        /// <returns>IDbCommand instance.</returns>
        protected virtual IDbCommand ReadCommand(params IExpression[] parameters)
        {
            Select query = new Select(linkMapper.Table["**"], modelMapper.Table["*"])
            {
                FROM =
                {
                    modelMapper.Table.INNERJOIN(
                        linkMapper.Table,
                        modelMapper.Table["Id"] == linkMapper.Table[this.joinField])
                }
            };
            foreach (IExpression expression in parameters)
            {
                query.WHERE.Add(expression);
            }
            return this.database.GetCommand(query);
        }

        public override L ReadObject(IDataReader reader)
        {
            throw new NotImplementedException();
        }

        protected override List<IDbCommand> UpdateCommands(L obj)
        {
            throw new NotImplementedException();
        }

        protected override List<IDbCommand> InsertCommands(L obj)
        {
            throw new NotImplementedException();
        }

        protected override List<IDbCommand> DeleteCommands(L obj)
        {
            throw new NotImplementedException();
        }
    }
}
