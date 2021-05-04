using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SQLService
{
    sealed class SQL_Service
    {
        private string _connectionString { get; set; }

        public SQL_Service(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreationConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public void CloseConnection(IDbConnection connection)
        {
            var sqlConnection = (SqlConnection)connection;
            sqlConnection.Close();
            sqlConnection.Dispose();
        }

        public SqlCommand CreateStoredProcedureCommand (SqlConnection connection, string storedProcedure)
        {
            SqlCommand cmd = new SqlCommand(storedProcedure, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            return cmd;
        }

        public SqlCommand CreateCommand(SqlConnection connection, string command)
        {
            SqlCommand cmd = new SqlCommand(command, connection);
            cmd.CommandType = CommandType.Text;

            return cmd;
        }

        public SqlCommand CreateStoredProcedureCommand(SqlConnection connection, string storedProcedure, SqlParameter[] sqlParameters)
        { 
            SqlCommand cmd = new SqlCommand(storedProcedure, connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(sqlParameters);

            return cmd;
        }

        public SqlDataReader ExecuteReader(SqlCommand cmd)
        {
            return cmd.ExecuteReader();
        }

        public List<T> GetObjectsFromExecuteReader<T>(SqlDataReader sqlDataReader)
        {
            ReflectionPopulator<T> generic = new ReflectionPopulator<T>();
            return generic.CreateList(sqlDataReader);
        }
    }

    sealed class ReflectionPopulator<T>
    {
        public List<T> CreateList(SqlDataReader reader)
        {
            var results = new List<T>();
            var properties = typeof(T).GetProperties();

            while (reader.Read())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var property in typeof(T).GetProperties())
                {
                    try
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                        {
                            Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            property.SetValue(item, Convert.ChangeType(reader[property.Name], convertTo), null);
                        }
                    }
                    catch (Exception ex)
                    {
                        // property.Name is undefined from the SQL return data. No action is required
                    }
                }
                results.Add(item);
            }
            return results;
        }
    }
}
