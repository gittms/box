using System;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Definitif;

namespace Definitif.VisualStudio.Generator
{
    public static partial class Extensions
    {
        /// <summary>
        /// Converts Model object to Mapper CodeType instance.
        /// </summary>
        public static CodeTypeDeclaration[] ToMapperCodeType(this Model model, CodeNamespace modelsNamespace)
        {
            CodeTypeDeclaration codeType = model.ToCodeType(false);
            codeType.BaseTypes.Add(String.Format("Mapper<{0}.{1}>", modelsNamespace.Name, model.Name));

            // Generating strings for values reading and assignement.
            List<string> values = new List<string>(),
                reads = new List<string>(),
                readsWithPrefix = new List<string>();
            foreach (Member member in model.Members)
            {
                if (!member.IsMapped) continue;

                var replacement = new
                {
                    column = member.ColumnName,
                    cast = member.ColumnCastingType,
                    type = member.Type,
                    name = member.Name,
                };

                // When member type is Id:
                //   INSERT:
                //     m.C.Name == ((obj.Name == Id.Empty) ? null : obj.Name)
                //   SELECT:
                //     Name = (reader["column"] == DBNull.Value) ? Id.Empty : new Id(reader["column"])
                //     Name = (reader[fieldPrefix + "column"] == DBNull.Value) ? Id.Empty : new Id(reader[fieldPrefix + "column"])
                if (member.Type == "Id")
                {
                    values.Add(@"m.C.{name} == ((obj.{name} == Id.Empty) ? null : obj.{name}.Value)".F(replacement));
                    reads.Add(@"{name} = (reader[""{column}""] == DBNull.Value) ? Id.Empty : new Id(reader[""{column}""]),".F(replacement));
                    readsWithPrefix.Add(@"{name} = (reader[fieldPrefix + ""{column}""] == DBNull.Value) ? Id.Empty : new Id(reader[fieldPrefix + ""{column}""]),".F(replacement));
                }
                // When member is foreign key, so it is represented
                // by Id and typed properties:
                //   INSERT:
                //     m.C["NameId"] == ((obj.NameId == Id.Empty) ? null : obj.Name)
                //   SELECT:
                //     NameId = (reader["column"] == DBNull.Value) ? Id.Empty : new Id(reader["column"])
                //     NameId = (reader[fieldPrefix + "column"] == DBNull.Value) ? Id.Empty : new Id(reader[fieldPrefix + "column"])
                else if ((member.Modifiers & Modifier.Foreign_key) != 0)
                {
                    values.Add(@"m.C[""{column}""] == ((obj.{name}Id == Id.Empty) ? null : obj.{name}Id.Value)".F(replacement));
                    reads.Add(@"{name}Id = (reader[""{column}""] == DBNull.Value) ? Id.Empty : new Id(reader[""{column}""]),".F(replacement));
                    readsWithPrefix.Add(@"{name}Id = (reader[fieldPrefix + ""{column}""] == DBNull.Value) ? Id.Empty : new Id(reader[fieldPrefix + ""{column}""]),".F(replacement));
                }
                // When member have custom casting type specified:
                //   INSERT:
                //     m.C.Name == (cast)obj.Name
                //   SELECT:
                //     Name = (type)((cast)reader["column"])
                //          or (type)(reader["column"] as string)
                //          or (type)(reader["column"] as cast?)
                //     Name = (type)((cast)reader[fieldPrefix + "column"])
                //          or (type)(reader[fieldPrefix + "column"] as string)
                //          or (type)(reader[fieldPrefix + "column"] as cast?)
                else if (!String.IsNullOrWhiteSpace(member.ColumnCastingType))
                {
                    values.Add(@"m.C.{name} == ({cast})obj.{name}".F(replacement));
                    if (!replacement.cast.EndsWith("?") && replacement.cast != "string")
                    {
                        reads.Add(@"{name} = ({type})(({cast})reader[""{column}""]),".F(replacement));
                        readsWithPrefix.Add(@"{name} = ({type})(({cast})reader[fieldPrefix + ""{column}""]),".F(replacement));
                    }
                    else
                    {
                        reads.Add(@"{name} = ({type})(reader[""{column}""] as {cast}),".F(replacement));
                        readsWithPrefix.Add(@"{name} = ({type})(reader[fieldPrefix + ""{column}""] as {cast}),".F(replacement));
                    }
                }
                // When member is just a mapped member:
                //   INSERT:
                //     m.C.Name == obj.Name
                //   SELECT:
                //     Name = (type)reader["column"]
                //         or reader["column"] as string
                //         or reader["column"] as type?
                //     Name = (type)reader[fieldPrefix + "column"]
                //         or reader[fieldPrefix + "column"] as string
                //         or reader[fieldPrefix + "column"] as type?
                else
                {
                    values.Add(@"m.C.{name} == obj.{name}".F(replacement));
                    if (!replacement.type.EndsWith("?") && replacement.type != "string")
                    {
                        reads.Add(@"{name} = ({type})reader[""{column}""],".F(replacement));
                        readsWithPrefix.Add(@"{name} = ({type})reader[fieldPrefix + ""{column}""],".F(replacement));
                    }
                    else
                    {
                        reads.Add(@"{name} = reader[""{column}""] as {type},".F(replacement));
                        readsWithPrefix.Add(@"{name} = reader[fieldPrefix + ""{column}""] as {type},".F(replacement));
                    }
                }
            }

            string valuesStr = String.Join(" &" + Environment.NewLine, values.ToArray()).Indent(7 * 4).Trim(),
                valuesAndStr = (valuesStr != "") ? "&" + Environment.NewLine + "".PadLeft(7 * 4, ' ') + valuesStr : "";
            if (valuesStr == "") valuesStr = "null";
            var variables = new
            {
                modelNamespace = modelsNamespace.Name,
                type = model.Name,
                database = String.IsNullOrWhiteSpace(model.DatabaseRef) ? Constants.DefaultDatabase : model.DatabaseRef,
                table = model.TableName,
                values = valuesStr,
                valuesAnd = valuesAndStr,
                reads = String.Join(Environment.NewLine, reads.ToArray()).Indent(4 * 4).Trim(),
                readsWithPrefix = String.Join(Environment.NewLine, readsWithPrefix.ToArray()).Indent(4 * 4).Trim(),
            };

            // Mapper constructor initializes database and
            // table references.
            codeType.Members.Add(new CodeSnippetTypeMember(@"
        public {type}()
        {{
            this.database = global::{database};
            this.table = this.database[""{table}""];
        }}".F(variables) + Environment.NewLine));

            // Mapper reader method.
            codeType.Members.Add(new CodeSnippetTypeMember(@"
        public sealed override {modelNamespace}.{type} ReadObject(IDataReader reader)
        {{
            {modelNamespace}.{type} result = new {modelNamespace}.{type}()
            {{
                {reads}
            }};
            FillBase(result, reader);
            return result;
        }}
        public sealed override {modelNamespace}.{type} ReadObject(IDataReader reader, string fieldPrefix)
        {{
            {modelNamespace}.{type} result = new {modelNamespace}.{type}()
            {{
                {readsWithPrefix}
            }};
            FillBase(result, reader, fieldPrefix);
            return result;
        }}".F(variables) + Environment.NewLine));

            // Mapper INSERT command.
            codeType.Members.Add(new CodeSnippetTypeMember(@"
        protected sealed override List<DbCommand> InsertCommands({modelNamespace}.{type} obj) {{
            List<DbCommand> list = new List<DbCommand> {{
                this.database.GetCommand(
                    new Insert<{modelNamespace}.{type}>()
                        .Values(m =>
                            {values}))
            }};
            this.InsertCommandsExtension(obj, list);
            return list;
        }}
        partial void InsertCommandsExtension({modelNamespace}.{type} obj, List<DbCommand> list);"
            .F(variables) + Environment.NewLine));

            // Mapper UPDATE commands.
            codeType.Members.Add(new CodeSnippetTypeMember(@"
        protected sealed override List<DbCommand> UpdateCommands({modelNamespace}.{type} obj) {{
            List<DbCommand> list = new List<DbCommand> {{
                this.database.GetCommand(
                    new Update<{modelNamespace}.{type}>()
                        .Set(m =>
                            m.C.Version == obj.Version + 1 {valuesAnd})
                        .Where(m =>
                            m.C.Id == obj.Id.Value &
                            m.C.Version == obj.Version))
            }};
            this.UpdateCommandsExtension(obj, list);
            return list;
        }}
        partial void UpdateCommandsExtension({modelNamespace}.{type} obj, List<DbCommand> list);"
            .F(variables) + Environment.NewLine));

            // Mapper DELETE commands.
            codeType.Members.Add(new CodeSnippetTypeMember(@"
        protected sealed override List<DbCommand> DeleteCommands({modelNamespace}.{type} obj) {{
            List<DbCommand> list = new List<DbCommand> {{
                this.database.GetCommand(
                    new Delete<{modelNamespace}.{type}>()
                        .Where(m =>
                            m.C.Id == obj.Id.Value &
                            m.C.Version == obj.Version))
            }};
            this.DeleteCommandsExtension(obj, list);
            return list;
        }}
        partial void DeleteCommandsExtension({modelNamespace}.{type} obj, List<DbCommand> list);"
            .F(variables) + Environment.NewLine));

            return new CodeTypeDeclaration[] { codeType };
        }
    }
}
