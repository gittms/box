﻿using System;
using System.Data;
using System.Data.SqlClient;

namespace Definitif.Data.ObjectSql.Implementation.MsSql
{
    public class Database : ObjectSql.Database
    {
        protected override ObjectSql.Drawer GetDrawer()
        {
            return new Drawer();
        }

        protected override IDbConnection GetConnection()
        {
            return new SqlConnection(this.connectionString);
        }

        protected override IDbCommand GetCommand(IDbConnection Connection, string Command)
        {
            if (Connection.State == ConnectionState.Closed) Connection.Open();
            return new SqlCommand(Command, Connection as SqlConnection);
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
                    new Column(column["COLUMN_NAME"] as string));
            }

            connection.Close();
        }
    }
}
