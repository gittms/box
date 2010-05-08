using System;
using System.Data;
using System.Data.SqlClient;
using Definitif.Data;                                                                               

namespace Definitif.Data.ObjectSql.Implementation.MsSql
{
    public sealed class Database : Data.Database
    {
        protected override ObjectSql.Drawer GetDrawer()
        {
            return new Drawer();
        }

        public override IDbConnection GetConnection()
        {
            return new SqlConnection(this.connectionString);
        }

        public override IDbCommand GetCommand()
        {
            return new SqlCommand();
        }

        protected override void UpdateSchema()
        {
            SqlConnection connection = this.GetConnection() as SqlConnection;
            connection.Open();

            DataTable columns = connection.GetSchema("Columns");
            foreach (DataRow column in columns.Rows)
            {
                if (!this.tables.ContainsKey(column["TABLE_NAME"] as string))
                {
                    this.Add(new Table(column["TABLE_NAME"] as string));
                }
                this[column["TABLE_NAME"] as string].Add(
                    new Column(column["COLUMN_NAME"] as string,
                               column["DATA_TYPE"] as string));
            }

            connection.Close();
        }
    }
}
