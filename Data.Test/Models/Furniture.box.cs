//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30128.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Definitif;
using Definitif.Data;
using Definitif.Data.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;


namespace Definitif.Data.Test.Models {
    
    public partial class Table : Model<Definitif.Data.Test.Models.Mappers.Table> {

        /// <summary>
        /// Gets Table instance from database by given Id.
        /// </summary>
        public static Table Get(Id id) {
            return Table.Mapper.Read(id);
        }

        public class TableTableScheme : ModelTableScheme<Definitif.Data.Test.Models.Mappers.Table> {
            public TableTableScheme() {
                
            }

            private Column p_name = table["Name"];
            public Column Name { get { return p_name; } }
        }

        private static TableTableScheme tableScheme = new TableTableScheme();
        /// <summary>
        /// Gets Table model table scheme.
        /// </summary>
        public new TableTableScheme C {
            get {
                return tableScheme;
            }
        }

        [DataMember(Name = "Name", IsRequired = false)]
        public string Name {
            get { return p_name; }
            set { p_name = value; }
        }
        protected string p_name;

    }
    public partial class Chair : Model<Definitif.Data.Test.Models.Mappers.Chair> {

        /// <summary>
        /// Gets Chair instance from database by given Id.
        /// </summary>
        public static Chair Get(Id id) {
            return Chair.Mapper.Read(id);
        }

        public class ChairTableScheme : ModelTableScheme<Definitif.Data.Test.Models.Mappers.Chair> {
            public ChairTableScheme() {
                if (!p_table.Id.ForeignKeys.Contains(table["TableId"])) p_table.Id.ForeignKeys.Add(table["TableId"]);
            }

            private Table.TableTableScheme p_table = new Table().C;
            public Table.TableTableScheme Table { get { return p_table; } }
            private Column p_name = table["Name"];
            public Column Name { get { return p_name; } }
        }

        private static ChairTableScheme tableScheme = new ChairTableScheme();
        /// <summary>
        /// Gets Chair model table scheme.
        /// </summary>
        public new ChairTableScheme C {
            get {
                return tableScheme;
            }
        }

        [DataMember(Name = "Table", IsRequired = false)]
        private Int64 TableIdSurrogate {
            get { return (Int64)id_table.Value; }
            set { this.id_table = new Id(value); }
        }

        internal Id TableId {
            get { return id_table; }
            set { id_table = value; }
        }
        protected Id id_table = Id.Empty;

        public Table Table {
            get {
                if (p_table == null && id_table != Id.Empty) {
                    p_table = Table.Get(id_table);
                }
                return p_table;
            }
            set {
                p_table = value;
                id_table = value.Id;
            }
        }
        protected Table p_table = null;

        [DataMember(Name = "Name", IsRequired = false)]
        public string Name {
            get { return p_name; }
            set { p_name = value; }
        }
        protected string p_name;

    }
    public partial class Table {

        /// <summary>
        /// Gets linked Chair objects.
        /// </summary>
        public Chair[] GetChairs() {
            return new Select<Chair>().Where(m => m.C.Table.Id == this.Id).Read();
        }

    }
    public partial class ChairToChair : Model<Definitif.Data.Test.Models.Mappers.ChairToChair>, IManyToMany {

        /// <summary>
        /// Gets ChairToChair instance from database by given Id.
        /// </summary>
        public static ChairToChair Get(Id id) {
            return ChairToChair.Mapper.Read(id);
        }

        public class ChairToChairTableScheme : ModelTableScheme<Definitif.Data.Test.Models.Mappers.ChairToChair> {
            public ChairToChairTableScheme() {
                if (!p_first.Id.ForeignKeys.Contains(table["FirstId"])) p_first.Id.ForeignKeys.Add(table["FirstId"]);
                if (!p_second.Id.ForeignKeys.Contains(table["SecondId"])) p_second.Id.ForeignKeys.Add(table["SecondId"]);
            }

            private Chair.ChairTableScheme p_first = new Chair().C;
            public Chair.ChairTableScheme First { get { return p_first; } }
            private Chair.ChairTableScheme p_second = new Chair().C;
            public Chair.ChairTableScheme Second { get { return p_second; } }
            private Column p_owner = table["Owner"];
            public Column Owner { get { return p_owner; } }
        }

        private static ChairToChairTableScheme tableScheme = new ChairToChairTableScheme();
        /// <summary>
        /// Gets ChairToChair model table scheme.
        /// </summary>
        public new ChairToChairTableScheme C {
            get {
                return tableScheme;
            }
        }

        [DataMember(Name = "First", IsRequired = false)]
        private Int64 FirstIdSurrogate {
            get { return (Int64)id_first.Value; }
            set { this.id_first = new Id(value); }
        }

        internal Id FirstId {
            get { return id_first; }
            set { id_first = value; }
        }
        protected Id id_first = Id.Empty;

        public Chair First {
            get {
                if (p_first == null && id_first != Id.Empty) {
                    p_first = Chair.Get(id_first);
                }
                return p_first;
            }
            set {
                p_first = value;
                id_first = value.Id;
            }
        }
        protected Chair p_first = null;

        [DataMember(Name = "Second", IsRequired = false)]
        private Int64 SecondIdSurrogate {
            get { return (Int64)id_second.Value; }
            set { this.id_second = new Id(value); }
        }

        internal Id SecondId {
            get { return id_second; }
            set { id_second = value; }
        }
        protected Id id_second = Id.Empty;

        public Chair Second {
            get {
                if (p_second == null && id_second != Id.Empty) {
                    p_second = Chair.Get(id_second);
                }
                return p_second;
            }
            set {
                p_second = value;
                id_second = value.Id;
            }
        }
        protected Chair p_second = null;

        [DataMember(Name = "Owner", IsRequired = false)]
        public string Owner {
            get { return p_owner; }
            set { p_owner = value; }
        }
        protected string p_owner;

    }
    public static partial class ChairToChairExtensions {

        /// <summary>
        /// Gets linked Chair objects.
        /// </summary>
        public static ManyToMany<ChairToChair, Chair>[] GetChairs(this Chair key) {
            return ManyToMany<ChairToChair, Chair>.Mapper.Get(key.Id);
        }

    }
}
namespace Definitif.Data.Test.Models.Mappers {
    
    public partial class Table : Mapper<Definitif.Data.Test.Models.Table> {

        public Table()
        {
            this.database = global::Core.Database;
            this.table = this.database["Tables"];
        }

        public sealed override Definitif.Data.Test.Models.Table ReadObject(IDataReader reader)
        {
            Definitif.Data.Test.Models.Table result = new Definitif.Data.Test.Models.Table()
            {
                Name = (string)reader["Name"],
            };
            FillBase(result, reader);
            return result;
        }
        public sealed override Definitif.Data.Test.Models.Table ReadObject(IDataReader reader, string fieldPrefix)
        {
            Definitif.Data.Test.Models.Table result = new Definitif.Data.Test.Models.Table()
            {
                Name = (string)reader[fieldPrefix + "Name"],
            };
            FillBase(result, reader, fieldPrefix);
            return result;
        }

        protected sealed override List<DbCommand> InsertCommands(Definitif.Data.Test.Models.Table obj) {
            List<DbCommand> list = new List<DbCommand> {
                this.database.GetCommand(
                    new Insert<Definitif.Data.Test.Models.Table>()
                        .Values(m =>
                            m.C.Name == obj.Name))
            };
            this.InsertCommandsExtension(obj, list);
            return list;
        }
        partial void InsertCommandsExtension(Definitif.Data.Test.Models.Table obj, List<DbCommand> list);

        protected sealed override List<DbCommand> UpdateCommands(Definitif.Data.Test.Models.Table obj) {
            List<DbCommand> list = new List<DbCommand> {
                this.database.GetCommand(
                    new Update<Definitif.Data.Test.Models.Table>()
                        .Set(m =>
                            m.C.Version == obj.Version + 1 &
                            m.C.Name == obj.Name)
                        .Where(m =>
                            m.C.Id == obj.Id.Value &
                            m.C.Version == obj.Version))
            };
            this.UpdateCommandsExtension(obj, list);
            return list;
        }
        partial void UpdateCommandsExtension(Definitif.Data.Test.Models.Table obj, List<DbCommand> list);

        protected sealed override List<DbCommand> DeleteCommands(Definitif.Data.Test.Models.Table obj) {
            List<DbCommand> list = new List<DbCommand> {
                this.database.GetCommand(
                    new Delete<Definitif.Data.Test.Models.Table>()
                        .Where(m =>
                            m.C.Id == obj.Id.Value &
                            m.C.Version == obj.Version))
            };
            this.DeleteCommandsExtension(obj, list);
            return list;
        }
        partial void DeleteCommandsExtension(Definitif.Data.Test.Models.Table obj, List<DbCommand> list);

    }
    public partial class Chair : Mapper<Definitif.Data.Test.Models.Chair> {

        public Chair()
        {
            this.database = global::Core.Database;
            this.table = this.database["Chairs"];
        }

        public sealed override Definitif.Data.Test.Models.Chair ReadObject(IDataReader reader)
        {
            Definitif.Data.Test.Models.Chair result = new Definitif.Data.Test.Models.Chair()
            {
                TableId = (reader["TableId"] == DBNull.Value) ? Id.Empty : new Id(reader["TableId"]),
                Name = (string)reader["Name"],
            };
            FillBase(result, reader);
            return result;
        }
        public sealed override Definitif.Data.Test.Models.Chair ReadObject(IDataReader reader, string fieldPrefix)
        {
            Definitif.Data.Test.Models.Chair result = new Definitif.Data.Test.Models.Chair()
            {
                TableId = (reader[fieldPrefix + "TableId"] == DBNull.Value) ? Id.Empty : new Id(reader[fieldPrefix + "TableId"]),
                Name = (string)reader[fieldPrefix + "Name"],
            };
            FillBase(result, reader, fieldPrefix);
            return result;
        }

        protected sealed override List<DbCommand> InsertCommands(Definitif.Data.Test.Models.Chair obj) {
            List<DbCommand> list = new List<DbCommand> {
                this.database.GetCommand(
                    new Insert<Definitif.Data.Test.Models.Chair>()
                        .Values(m =>
                            m.C["TableId"] == ((obj.TableId == Id.Empty) ? null : obj.TableId.Value) &
                            m.C.Name == obj.Name))
            };
            this.InsertCommandsExtension(obj, list);
            return list;
        }
        partial void InsertCommandsExtension(Definitif.Data.Test.Models.Chair obj, List<DbCommand> list);

        protected sealed override List<DbCommand> UpdateCommands(Definitif.Data.Test.Models.Chair obj) {
            List<DbCommand> list = new List<DbCommand> {
                this.database.GetCommand(
                    new Update<Definitif.Data.Test.Models.Chair>()
                        .Set(m =>
                            m.C.Version == obj.Version + 1 &
                            m.C["TableId"] == ((obj.TableId == Id.Empty) ? null : obj.TableId.Value) &
                            m.C.Name == obj.Name)
                        .Where(m =>
                            m.C.Id == obj.Id.Value &
                            m.C.Version == obj.Version))
            };
            this.UpdateCommandsExtension(obj, list);
            return list;
        }
        partial void UpdateCommandsExtension(Definitif.Data.Test.Models.Chair obj, List<DbCommand> list);

        protected sealed override List<DbCommand> DeleteCommands(Definitif.Data.Test.Models.Chair obj) {
            List<DbCommand> list = new List<DbCommand> {
                this.database.GetCommand(
                    new Delete<Definitif.Data.Test.Models.Chair>()
                        .Where(m =>
                            m.C.Id == obj.Id.Value &
                            m.C.Version == obj.Version))
            };
            this.DeleteCommandsExtension(obj, list);
            return list;
        }
        partial void DeleteCommandsExtension(Definitif.Data.Test.Models.Chair obj, List<DbCommand> list);

    }
    public partial class ChairToChair : Mapper<Definitif.Data.Test.Models.ChairToChair>, IManyToManyMapper {

        public ChairToChair()
        {
            this.database = global::Core.Database;
            this.table = this.database["Chair2Chair"];
        }

        public sealed override Definitif.Data.Test.Models.ChairToChair ReadObject(IDataReader reader)
        {
            Definitif.Data.Test.Models.ChairToChair result = new Definitif.Data.Test.Models.ChairToChair()
            {
                FirstId = (reader["FirstId"] == DBNull.Value) ? Id.Empty : new Id(reader["FirstId"]),
                SecondId = (reader["SecondId"] == DBNull.Value) ? Id.Empty : new Id(reader["SecondId"]),
                Owner = (string)reader["Owner"],
            };
            FillBase(result, reader);
            return result;
        }
        public sealed override Definitif.Data.Test.Models.ChairToChair ReadObject(IDataReader reader, string fieldPrefix)
        {
            Definitif.Data.Test.Models.ChairToChair result = new Definitif.Data.Test.Models.ChairToChair()
            {
                FirstId = (reader[fieldPrefix + "FirstId"] == DBNull.Value) ? Id.Empty : new Id(reader[fieldPrefix + "FirstId"]),
                SecondId = (reader[fieldPrefix + "SecondId"] == DBNull.Value) ? Id.Empty : new Id(reader[fieldPrefix + "SecondId"]),
                Owner = (string)reader[fieldPrefix + "Owner"],
            };
            FillBase(result, reader, fieldPrefix);
            return result;
        }

        protected sealed override List<DbCommand> InsertCommands(Definitif.Data.Test.Models.ChairToChair obj) {
            List<DbCommand> list = new List<DbCommand> {
                this.database.GetCommand(
                    new Insert<Definitif.Data.Test.Models.ChairToChair>()
                        .Values(m =>
                            m.C["FirstId"] == ((obj.FirstId == Id.Empty) ? null : obj.FirstId.Value) &
                            m.C["SecondId"] == ((obj.SecondId == Id.Empty) ? null : obj.SecondId.Value) &
                            m.C.Owner == obj.Owner))
            };
            this.InsertCommandsExtension(obj, list);
            return list;
        }
        partial void InsertCommandsExtension(Definitif.Data.Test.Models.ChairToChair obj, List<DbCommand> list);

        protected sealed override List<DbCommand> UpdateCommands(Definitif.Data.Test.Models.ChairToChair obj) {
            List<DbCommand> list = new List<DbCommand> {
                this.database.GetCommand(
                    new Update<Definitif.Data.Test.Models.ChairToChair>()
                        .Set(m =>
                            m.C.Version == obj.Version + 1 &
                            m.C["FirstId"] == ((obj.FirstId == Id.Empty) ? null : obj.FirstId.Value) &
                            m.C["SecondId"] == ((obj.SecondId == Id.Empty) ? null : obj.SecondId.Value) &
                            m.C.Owner == obj.Owner)
                        .Where(m =>
                            m.C.Id == obj.Id.Value &
                            m.C.Version == obj.Version))
            };
            this.UpdateCommandsExtension(obj, list);
            return list;
        }
        partial void UpdateCommandsExtension(Definitif.Data.Test.Models.ChairToChair obj, List<DbCommand> list);

        protected sealed override List<DbCommand> DeleteCommands(Definitif.Data.Test.Models.ChairToChair obj) {
            List<DbCommand> list = new List<DbCommand> {
                this.database.GetCommand(
                    new Delete<Definitif.Data.Test.Models.ChairToChair>()
                        .Where(m =>
                            m.C.Id == obj.Id.Value &
                            m.C.Version == obj.Version))
            };
            this.DeleteCommandsExtension(obj, list);
            return list;
        }
        partial void DeleteCommandsExtension(Definitif.Data.Test.Models.ChairToChair obj, List<DbCommand> list);

        public string FieldNameJoin(IModel model) {
            if (model is Definitif.Data.Test.Models.Chair) {
                return "FirstId";
            }
            else if (model is Definitif.Data.Test.Models.Chair) {
                return "SecondId";
            }
            else throw new ArgumentException();
        }

        public string FieldNameWhere(IModel model) {
            if (model is Definitif.Data.Test.Models.Chair) {
                return "FirstId";
            }
            else if (model is Definitif.Data.Test.Models.Chair) {
                return "SecondId";
            }
            else throw new ArgumentException();
        }

    }
}
