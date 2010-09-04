using System;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using Definitif.Data;

namespace Definitif.Data.Providers.MySql
{
    public sealed class Database : Data.Database
    {
        protected override Queries.Drawer GetDrawer()
        {
            return new MySql.Drawer();
        }

        protected override DbConnection GetDatabaseConnection()
        {
            return new MySqlConnection(this.ConnectionString);
        }

        public override DbCommand GetCommand()
        {
            return new MySqlCommand();
        }

        protected override void UpdateSchema()
        {
            MySqlConnection connection = this.GetConnection() as MySqlConnection;
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
