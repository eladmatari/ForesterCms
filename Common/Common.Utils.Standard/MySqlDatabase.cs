using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Common.Utils.Standard
{
    public class MySqlDatabase : IDisposable
    {
        public MySqlConnection Connection;

        public MySqlDatabase(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
            this.Connection.Open();
        }

        public void Dispose()
        {
            Connection.Close();
        }

        public DataTable ExecuteDataTable(string query, Action<MySqlCommand> setCommand = null)
        {
            using (var cmd = Connection.CreateCommand())
            {
                cmd.CommandText = query;
                setCommand?.Invoke(cmd);
                using (var reader = cmd.ExecuteReader())
                {
                    var schema = reader.GetSchemaTable();

                    var dt = new DataTable();
                    dt.TableName = "Table";

                    foreach (DataColumn column in schema.Columns)
                    {
                        dt.Columns.Add(column.ColumnName, column.DataType);
                    }

                    while (reader.Read())
                    {
                        var row = dt.NewRow();

                        for (int i = 0; i < schema.Columns.Count; i++)
                        {
                            row[schema.Columns[i].ColumnName] = reader.GetValue(i);
                        }

                        dt.Rows.Add(row);
                    }

                    return dt;
                }
            }
        }

        public void ExecuteNonQuery(string query, Action<MySqlCommand> setCommand = null)
        {
            ExecuteDataTable(query, setCommand);
        }

        public object ExecuteScalar(string query, Action<MySqlCommand> setCommand = null)
        {
            var dt = ExecuteDataTable(query, setCommand);

            if (dt.Rows.Count > 0)
                return dt.Rows[0][0];

            return null;
        }
    }
}
