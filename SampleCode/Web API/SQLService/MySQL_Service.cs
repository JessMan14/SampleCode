using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SQLService
{
    public class MySQL_Service
    {
        static ILog _sqlLog = LogManager.GetLogger(typeof(MySQL_Service));

        public static List<T> GetObjectsFromExecuteReader<T>(MySqlDataReader mySqlDataReader)
        {
            var results = new List<T>();
            var properties = typeof(T).GetProperties();

            while (mySqlDataReader.Read())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var property in typeof(T).GetProperties())
                {
                    try
                    {
                        if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal(property.Name)))
                        {
                            Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            property.SetValue(item, Convert.ChangeType(mySqlDataReader[property.Name], convertTo), null);
                        }
                    }
                    catch (Exception ex)
                    {
                        _sqlLog.Debug("***GetObjectsFromExecuteReader: Mapping issue. Property.Name = " + property.Name);
                    }
                }
                results.Add(item);
            }
            return results;
        }

        public static List<T> GetItems<T>(string connectionString, string commandText)
        {
            string mySQLCommandText = "call " + commandText + ";";
            _sqlLog.Debug("GetItems. CommandText = " + mySQLCommandText);
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = mySQLCommandText;

                    using (var rdr = command.ExecuteReader())
                    {
                        List<T> items = GetObjectsFromExecuteReader<T>(rdr);
                        connection.Close();

                        return items;
                    }
                }
            }
        }


        public static List<T> GetItems<T>(string connectionString, string commandText, string parameters)
        {
            string mySQLCommandText = "call " + commandText + "(" + parameters + ")" + ";";
            _sqlLog.Debug("GetItems with parms. CommandText = " + mySQLCommandText);
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = mySQLCommandText;

                    using (var rdr = command.ExecuteReader())
                    {
                        List<T> items = GetObjectsFromExecuteReader<T>(rdr);
                        connection.Close();

                        return items;
                    }
                }
            }
        }

        public static List<T> GetItemsByCommand<T>(string connectionString, string commandText)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = commandText;
                    _sqlLog.Debug("GetItemsByCommand. CommandText = " + commandText);
                    using (var rdr = command.ExecuteReader())
                    {
                        List<T> items = GetObjectsFromExecuteReader<T>(rdr);
                        connection.Close();

                        return items;
                    }
                }
            }
        }

        public static List<T> GetItemsByCommand<T>(string connectionString, string commandText, List<SqlParameter> sqlParameters)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = commandText;
                    foreach (var item in sqlParameters)
                    {
                        command.Parameters.Add(new MySqlParameter(item.ParameterName, item.Value));
                    }

                    _sqlLog.Debug("GetItemsByCommand with params. CommandText = " + commandText);
                    using (var rdr = command.ExecuteReader())
                    {
                        List<T> items = GetObjectsFromExecuteReader<T>(rdr);
                        connection.Close();

                        return items;
                    }
                }
            }
        }

        public static T GetItemByCommand<T>(string connectionString, string commandText, List<SqlParameter> sqlParameters)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = commandText;
                    foreach (var item in sqlParameters)
                    {
                        command.Parameters.Add(new MySqlParameter(item.ParameterName, item.Value));
                    }

                    _sqlLog.Debug("GetItemByCommand with params. CommandText = " + commandText);
                    using (var rdr = command.ExecuteReader())
                    {
                        List<T> items = GetObjectsFromExecuteReader<T>(rdr);
                        connection.Close();

                        return items.FirstOrDefault();
                    }
                }
            }
        }

        public static T GetItem<T>(string connectionString, string commandText, string parmater)
        {
            string mySQLCommandText = "call " + commandText + "(" + parmater + ")"  + ";";    // call spGetCustomers;
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = mySQLCommandText;
                    _sqlLog.Debug("GetItem. CommandText = " + mySQLCommandText);
                    using (var rdr = command.ExecuteReader())
                    {
                        List<T> items = GetObjectsFromExecuteReader<T>(rdr);
                        connection.Close();

                        return items.FirstOrDefault();
                    }
                }
            }
        }

        public static object GetGenericItemByCommand(string connectionString, string commandText, List<SqlParameter> sqlParameters)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = commandText;
                    foreach (var item in sqlParameters)
                    {
                        command.Parameters.Add(new MySqlParameter(item.ParameterName, item.Value));
                    }

                    _sqlLog.Debug("GetGenericItemByCommand with params. CommandText = " + commandText);
                    var result = command.ExecuteScalar();
                    connection.Close();

                    return result;
                }
            }
        }

        public static void PerformCommand(string connectionString, string commandText)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = commandText;
                    _sqlLog.Debug("PerformCommand. CommandText = " + commandText);
                    command.ExecuteReader();
                    connection.Close();
                }
            }
        }

        internal static void PerformCommand(string connectionString, string commandText, List<SqlParameter> sqlParameters)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = commandText;
                    foreach (var item in sqlParameters)
                    {
                        command.Parameters.Add(new MySqlParameter(item.ParameterName, item.Value));
                    }
                    _sqlLog.Debug("PerformCommand. CommandText = " + commandText);
                    command.ExecuteReader();
                    connection.Close();
                }
            }
        }


        public static long PerformCommandWithAutoincrementedId(string connectionString, string commandText)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = commandText;

                    _sqlLog.Debug("PerformCommand. CommandText = " + commandText);
                    command.ExecuteReader();
                    long id = command.LastInsertedId;
                    connection.Close();
                    return id;
                }
            }
        }

        public static long PerformCommandWithAutoincrementedId(string connectionString, string commandText, List<SqlParameter> sqlParameters)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = commandText;
                    foreach (var item in sqlParameters)
                    {
                        command.Parameters.Add(new MySqlParameter(item.ParameterName, item.Value));
                    }
                    _sqlLog.Debug("PerformCommand. CommandText = " + commandText);
                    command.ExecuteReader();
                    long id = command.LastInsertedId;
                    connection.Close();
                    return id;
                }
            }
        }

        public static bool ValidateCommand<T>(string connectionString, string commandText)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = commandText;
                    _sqlLog.Debug("PerformCommand. CommandText = " + commandText);
                    using (var rdr = command.ExecuteReader())
                    {
                        List<T> items = GetObjectsFromExecuteReader<T>(rdr);
                        connection.Close();
                        return items.Count > 0;
                    }
                }
            }
        }

        public static T GetRecord<T>(string connectionString, string commandText)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = commandText;
                    _sqlLog.Debug("GetItem. CommandText = " + commandText);
                    using (var rdr = command.ExecuteReader())
                    {
                        List<T> items = GetObjectsFromExecuteReader<T>(rdr);
                        connection.Close();

                        return items.FirstOrDefault();
                    }
                }
            }
        }

        public static DataTable GetItemsAsyncDataTable(string connectionString, string storedProcedure, List<SqlParameter> sqlParameters)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = new MySqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (var param in sqlParameters)
                    {
                        MySqlParameter item = new MySqlParameter();
                        item.Direction = ParameterDirection.Input;
                        item.DbType = param.DbType;
                        item.ParameterName = param.ParameterName;
                        item.Value = param.Value;
                        command.Parameters.Add(item);
                    }
                    
                    using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        dataAdapter.Fill(dt);

                        return dt;
                    }
                }
            }
        }

        public static void PerformSPCommand(string connectionString, string storedProcedure, List<SqlParameter> sqlParameters)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = new MySqlCommand(storedProcedure, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (var param in sqlParameters)
                    {
                        MySqlParameter item = new MySqlParameter();
                        item.Direction = ParameterDirection.Input;
                        item.DbType = param.DbType;
                        item.ParameterName = param.ParameterName;
                        item.Value = param.Value;
                        command.Parameters.Add(item);
                    }

                    _sqlLog.Debug("PerformSPCommand. CommandText = " + storedProcedure);
                    command.ExecuteReader();
                    connection.Close();
                }
            }
        }

        public static int PerformSPCommandWithAutoincrementedId(string connectionString, string storedProcedure, List<SqlParameter> sqlParameters)
        {
            int result = 0;

            using (var connection = new MySqlConnection(connectionString))
            {
                using (var command = new MySqlCommand(storedProcedure, connection))
                {
                    connection.Open();
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (var param in sqlParameters)
                    {
                        MySqlParameter item = new MySqlParameter();
                        item.Direction = ParameterDirection.Input;
                        item.DbType = param.DbType;
                        item.ParameterName = param.ParameterName;
                        item.Value = param.Value;
                        command.Parameters.Add(item);
                    }

                    MySqlParameter output = new MySqlParameter();
                    output.Direction = ParameterDirection.Output;
                    output.ParameterName = "@return_id";
                    command.Parameters.Add(output);

                    _sqlLog.Debug("PerformSPCommandWithAutoIncrementId. CommandText = " + storedProcedure);
                    command.ExecuteScalar();

                    result = (int)output.Value;

                    connection.Close();
                }
            }

            return result;
        }
    }
}
