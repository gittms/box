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
        public static CodeTypeDeclaration ToMapperCodeType(this Model model, CodeNamespace modelsNamespace)
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
                //     table["column"] == (obj.Name == Id.Empty) ? DBNull.Value : obj.Name
                //   SELECT:
                //     Name = (reader["column"] == DBNull.Value) ? Id.Empty : new Id(reader["column"])
                //     Name = (reader[fieldPrefix + "column"] == DBNull.Value) ? Id.Empty : new Id(reader[fieldPrefix + "column"])
                if (member.Type == "Id")
                {
                    values.Add(@"table[""{column}""] == (obj.{name} == Id.Empty) ? DBNull.Value : obj.{name},".F(replacement));
                    reads.Add(@"{name} = (reader[""{column}""] == DBNull.Value) ? Id.Empty : new Id(reader[""{column}""]),".F(replacement));
                    readsWithPrefix.Add(@"{name} = (reader[fieldPrefix + ""{column}""] == DBNull.Value) ? Id.Empty : new Id(reader[fieldPrefix + ""{column}""]),".F(replacement));
                }
                // When member is foreign key, so it is represented
                // by Id and typed properties:
                //   INSERT:
                //     table["column"] == (obj.NameId == Id.Empty) ? DBNull.Value : obj.Name
                //   SELECT:
                //     NameId = (reader["column"] == DBNull.Value) ? Id.Empty : new Id(reader["column"])
                //     NameId = (reader[fieldPrefix + "column"] == DBNull.Value) ? Id.Empty : new Id(reader[fieldPrefix + "column"])
                else if ((member.Modifiers & Modifier.Foreign_key) != 0)
                {
                    values.Add(@"table[""{column}""] == (obj.{name}Id == Id.Empty) ? DBNull.Value : obj.{name},".F(replacement));
                    reads.Add(@"{name}Id = (reader[""{column}""] == DBNull.Value) ? Id.Empty : new Id(reader[""{column}""]),".F(replacement));
                    readsWithPrefix.Add(@"{name}Id = (reader[fieldPrefix + ""{column}""] == DBNull.Value) ? Id.Empty : new Id(reader[fieldPrefix + ""{column}""]),".F(replacement));
                }
                // When member have custom casting type specified:
                //   INSERT:
                //     table["column"] == (cast)obj.Name
                //   SELECT:
                //     Name = (type)((cast)reader["column"])
                //     Name = (type)((cast)reader[fieldPrefix + "column"])
                else if (!String.IsNullOrWhiteSpace(member.ColumnCastingType))
                {
                    values.Add(@"table[""{column}""] == ({cast})obj.{name},".F(replacement));
                    reads.Add(@"{name} = ({type})(({cast})reader[""{column}""]),".F(replacement));
                    readsWithPrefix.Add(@"{name} = ({type})(({cast})reader[fieldPrefix + ""{column}""]),".F(replacement));
                }
                // When member is just a mapped member:
                //   INSERT:
                //     table["column"] == obj.Name
                //   SELECT:
                //     Name = (type)reader["column"]
                //     Name = (type)reader[fieldPrefix + "column"]
                else
                {
                    values.Add(@"table[""{column}""] == obj.{name},".F(replacement));
                    reads.Add(@"{name} = ({type})reader[""{column}""],".F(replacement));
                    readsWithPrefix.Add(@"{name} = ({type})reader[fieldPrefix + ""{column}""],".F(replacement));
                }
            }

            var variables = new
            {
                modelNamespace = modelsNamespace.Name,
                type = model.Name,
                database = String.IsNullOrWhiteSpace(model.DatabaseRef) ? Constants.DefaultDatabase : model.DatabaseRef,
                table = model.TableName,
                values = String.Join(Environment.NewLine, values.ToArray()).Indent(7 * 4).Trim(),
                reads = String.Join(Environment.NewLine, reads.ToArray()).Indent(4 * 4).Trim(),
                readsWithPrefix = String.Join(Environment.NewLine, readsWithPrefix.ToArray()).Indent(4 * 4).Trim(),
            };

            // Mapper constructor initializes database and
            // table references.
            codeType.Members.Add(new CodeSnippetTypeMember(@"
        public {type}()
        {{
            this.database = {database};
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
        protected sealed override List<IDbCommand> InsertCommands({modelNamespace}.{type} obj) {{
            List<IDbCommand> list = new List<IDbCommand> {{
                this.database.GetCommand(
                    new Insert() {{
                        VALUES = {{
                            {values}
                        }}
                    }})
            }};
            this.InsertCommandsExtension(obj, list);
            return list;
        }}
        partial void InsertCommandsExtension({modelNamespace}.{type} obj, List<IDbCommand> list);"
            .F(variables) + Environment.NewLine));

            // Mapper UPDATE commands.
            codeType.Members.Add(new CodeSnippetTypeMember(@"
        protected sealed override List<IDbCommand> UpdateCommands({modelNamespace}.{type} obj) {{
            List<IDbCommand> list = new List<IDbCommand> {{
                this.database.GetCommand(
                    new Update() {{
                        VALUES = {{
                            table[""Version""] == obj.Version + 1,
                            {values}
                        }},
                        WHERE = {{
                            table[""Id""] == obj.Id.Value,
                            table[""Version""] == obj.Version,
                        }}
                    }})
            }};
            this.UpdateCommandsExtension(obj, list);
            return list;
        }}
        partial void UpdateCommandsExtension({modelNamespace}.{type} obj, List<IDbCommand> list);"
            .F(variables) + Environment.NewLine));

            // Mapper DELETE commands.
            codeType.Members.Add(new CodeSnippetTypeMember(@"
        protected sealed override List<IDbCommand> DeleteCommands({modelNamespace}.{type} obj) {{
            List<IDbCommand> list = new List<IDbCommand> {{
                this.database.GetCommand(
                    new Delete(table) {{
                        WHERE = {{
                            table[""Id""] == obj.Id.Value, 
                            table[""Version""] == obj.Version,
                        }}
                    }})
            }};
            this.DeleteCommandsExtension(obj, list);
            return list;
        }}
        partial void DeleteCommandsExtension({modelNamespace}.{type} obj, List<IDbCommand> list);"
            .F(variables) + Environment.NewLine));

            return codeType;
        }
    }
}
