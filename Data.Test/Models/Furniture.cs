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

        public string Name {
            get { return p_name; }
            set { p_name = value; }
        }
        protected string p_name;

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
                        .Values(m =>
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
                            m.C.Table.Id == ((obj.TableId == Id.Empty) ? DBNull.Value : obj.TableId.Value) &
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
                        .Values(m =>
                            m.C.Version == obj.Version + 1 &
                            m.C.Table.Id == ((obj.TableId == Id.Empty) ? DBNull.Value : obj.TableId.Value) &
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
}
