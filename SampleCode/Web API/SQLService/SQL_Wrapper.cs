using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SQLService
{
    public class SQL_Wrapper : ISQL_Wrapper
    {
        public Task<List<T>> GetItemsAsync<T>(string connectionstring, string storedProcedure)
        {
            return Task.Run(() =>
            {
                SQL_Service sql = new SQL_Service(connectionstring);
                List<T> items = null;

                using (IDbConnection db = sql.CreationConnection())
                {
                    db.Open();

                    SqlCommand cmd = sql.CreateStoredProcedureCommand((SqlConnection)db, storedProcedure);

                    SqlDataReader rdr = null;
                    using (rdr = sql.ExecuteReader(cmd))
                    {
                        items = sql.GetObjectsFromExecuteReader<T>(rdr);
                    }

                    sql.CloseConnection(db);
                }
                return items;
            });
        }


        public Task<List<T>> GetItemsAsync<T>(string connectionstring, string storedProcedure, List<SqlParameter> sqlParameters)
        {
            return Task.Run(() =>
            {
                SQL_Service sql = new SQL_Service(connectionstring);
                List<T> items = null;

                using (IDbConnection db = sql.CreationConnection())
                {
                    db.Open();

                    SqlCommand cmd = sql.CreateStoredProcedureCommand((SqlConnection)db, storedProcedure, sqlParameters.ToArray());

                    SqlDataReader rdr = null;
                    using (rdr = sql.ExecuteReader(cmd))
                    {
                        items = sql.GetObjectsFromExecuteReader<T>(rdr);
                    }

                    sql.CloseConnection(db);
                }
                return items;
            });
        }

        public Task<T> GetItemAsync<T>(string connectionstring, string storedProcedure, List<SqlParameter> sqlParameters)
        {
            return Task.Run(() =>
            {
                SQL_Service sql = new SQL_Service(connectionstring);
                List<T> items = null;

                using (IDbConnection db = sql.CreationConnection())
                {
                    db.Open();

                    SqlCommand cmd = sql.CreateStoredProcedureCommand((SqlConnection)db, storedProcedure, sqlParameters.ToArray());

                    SqlDataReader rdr = null;
                    using (rdr = sql.ExecuteReader(cmd))
                    {
                        items = sql.GetObjectsFromExecuteReader<T>(rdr);
                    }

                    sql.CloseConnection(db);
                }
                return items.FirstOrDefault();
            });
        }

        Task<List<T>> ISQL_Wrapper.GetItemsByCommandAsync<T>(string connectionstring, string command)
        {
            return Task.Run(() =>
            {
                SQL_Service sql = new SQL_Service(connectionstring);
                List<T> items = null;

                using (IDbConnection db = sql.CreationConnection())
                {
                    db.Open();

                    SqlCommand cmd = sql.CreateCommand((SqlConnection)db, command);

                    SqlDataReader rdr = null;
                    using (rdr = sql.ExecuteReader(cmd))
                    {
                        items = sql.GetObjectsFromExecuteReader<T>(rdr);
                    }

                    sql.CloseConnection(db);
                }
                return items;
            });
        }

        public Task<List<T>> GetItemsByCommandAsync<T>(string connectionString, string command, List<SqlParameter> parameters)
        {
            return Task.Run(() =>
            {
                SQL_Service sql = new SQL_Service(connectionString);
                List<T> items = null;

                using (IDbConnection db = sql.CreationConnection())
                {
                    db.Open();

                    SqlCommand cmd = sql.CreateCommand((SqlConnection)db, command);

                    cmd.Parameters.Add(parameters);

                    SqlDataReader rdr = null;
                    using (rdr = sql.ExecuteReader(cmd))
                    {
                        items = sql.GetObjectsFromExecuteReader<T>(rdr);
                    }

                    sql.CloseConnection(db);
                }
                return items;
            });
        }

        public Task PerformCommandAsync(string connectionstring, string command)
        {
            return Task.Run(() =>
            {
                SQL_Service sql = new SQL_Service(connectionstring);

                using (IDbConnection db = sql.CreationConnection())
                {
                    db.Open();

                    SqlCommand cmd = sql.CreateCommand((SqlConnection)db, command);

                    SqlDataReader rdr = null;
                    sql.ExecuteReader(cmd);
                    sql.CloseConnection(db);
                }
                return;
            });
        }

        public Task<bool> ValidateCommandAsync<T>(string connectionstring, string command)
        {
            throw new System.NotImplementedException();
        }

        public Task<T> GetRecord<T>(string connectionString, string commandText)
        {
            throw new System.NotImplementedException();
        }

        public Task<long> PerformCommandWithAutoincrementedId(string connectionstring, string command)
        {
            throw new NotImplementedException();
        }

        public Task PerformCommandAsync(string connectionstring, string command, List<SqlParameter> sqlParameters)
        {
            throw new NotImplementedException();
        }

        public Task<long> PerformCommandWithAutoincrementedId(string connectionstring, string command, List<SqlParameter> sqlParameters)
        {
            throw new NotImplementedException();
        }

        public Task<DataTable> GetItemsDataTableAsync(string connectionString, string storedProcedure, List<SqlParameter> sqlParameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetItemByCommandAsync<T>(string connectionString, string command, List<SqlParameter> parameters)
        {
            throw new NotImplementedException();
        }

        Task<T> ISQL_Wrapper.GetItemByCommandAsync<T>(string connectionString, string command, List<SqlParameter> parameters)
        {
            throw new NotImplementedException();
        }

        public Task PerformSPCommandAsync(string connectionString, string storedProcedure, List<SqlParameter> parameters)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetGenericItemByCommandAsync(string connectionString, string command, List<SqlParameter> parameters)
        {
            throw new NotImplementedException();
        }

        public Task<int> PerformSPCommandWithAutoincrementedId(string connectionstring, string storedProcedure, List<SqlParameter> sqlParameters)
        {
            throw new NotImplementedException();
        }
    }
}
