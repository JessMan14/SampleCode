using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Data;

namespace SQLService
{
    public class MySQL_Wrapper : ISQL_Wrapper
    {
        public Task<List<T>> GetItemsAsync<T>(string connectionstring, string storedProcedure)
        {
            return Task.Run(() =>
            {
                return MySQL_Service.GetItems<T>(connectionstring, storedProcedure);
            });
        }

        public Task<List<T>> GetItemsAsync<T>(string connectionstring, string storedProcedure, List<SqlParameter> sqlParameters)
        {
            return Task.Run(() =>
            {
                return MySQL_Service.GetItems<T>(connectionstring, storedProcedure, GetParmeter(sqlParameters));
         
            });
        }

        public Task<T> GetItemAsync<T>(string connectionstring, string storedProcedure, List<SqlParameter> sqlParameters)
        {
            return Task.Run(() =>
            {
                return MySQL_Service.GetItem<T>(connectionstring, storedProcedure, GetParmeter(sqlParameters));
            });
        }

        public Task<List<T>> GetItemsByCommandAsync<T>(string connectionstring, string command)
        {
            return Task.Run(() =>
            {
                return MySQL_Service.GetItemsByCommand<T>(connectionstring, command);
            });
        }

        public Task PerformCommandAsync(string connectionstring, string command)
        {
            return Task.Run(() =>
            {
                MySQL_Service.PerformCommand(connectionstring, command);
            });
        }

        public Task PerformCommandAsync(string connectionstring, string command, List<SqlParameter> sqlParameters)
        {
            return Task.Run(() =>
            {
                MySQL_Service.PerformCommand(connectionstring, command, sqlParameters);
            });
        }

        public Task<bool> ValidateCommandAsync<T>(string connectionstring, string command)
        {
            return Task.Run(() =>
            {
                return MySQL_Service.ValidateCommand<T>(connectionstring, command);
            });
        }

        private string GetParmeter(List<SqlParameter> sqlParameters)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for(int i=0; i<sqlParameters.Count; i++)
            {
                stringBuilder.Append(sqlParameters[i].Value);
                if (i != sqlParameters.Count-1)
                {
                    stringBuilder.Append(",");
                }
            }

            return stringBuilder.ToString();
        }

        public Task<T> GetRecord<T>(string connectionString, string commandText)
        {
            return Task.Run(() =>
            {
                return MySQL_Service.GetRecord<T>(connectionString, commandText);
            });
        }

        public Task<long> PerformCommandWithAutoincrementedId(string connectionString, string commandText)
        {
            return Task.Run(() =>
            {
                return MySQL_Service.PerformCommandWithAutoincrementedId(connectionString, commandText);
            });
        }

        public Task<long> PerformCommandWithAutoincrementedId(string connectionString, string commandText, List<SqlParameter> sqlParameters)
        {
            return Task.Run(() =>
            {
                return MySQL_Service.PerformCommandWithAutoincrementedId(connectionString, commandText, sqlParameters);
            });
        }

        public Task<List<T>> GetItemsByCommandAsync<T>(string connectionString, string command, List<SqlParameter> parameters)
        {
            return Task.Run(() =>
            {
                return MySQL_Service.GetItemsByCommand<T>(connectionString, command, parameters);
            });
        }

        public Task<DataTable> GetItemsDataTableAsync(string connectionString, string storedProcedure, List<SqlParameter> sqlParameters)
        {
            return Task.Run(() =>
            {
                return MySQL_Service.GetItemsAsyncDataTable(connectionString, storedProcedure, sqlParameters);
            });
        }

        public Task<T> GetItemByCommandAsync<T>(string connectionString, string command, List<SqlParameter> parameters)
        {
            return Task.Run(() =>
            {
                return MySQL_Service.GetItemByCommand<T>(connectionString, command, parameters);
            });
        }

        public Task PerformSPCommandAsync(string connectionString, string storedProcedure, List<SqlParameter> parameters)
        {
            return Task.Run(() =>
            {
                MySQL_Service.PerformSPCommand(connectionString, storedProcedure, parameters);
            });
        }

        public Task<object> GetGenericItemByCommandAsync(string connectionString, string command, List<SqlParameter> parameters)
        {
            return Task.Run(() =>
            {
                return MySQL_Service.GetGenericItemByCommand(connectionString, command, parameters);
            });
        }

        public Task<int> PerformSPCommandWithAutoincrementedId(string connectionstring, string storedProcedure, List<SqlParameter> sqlParameters)
        {
            return Task.Run(() =>
            {
                return MySQL_Service.PerformSPCommandWithAutoincrementedId(connectionstring, storedProcedure, sqlParameters);
            });
        }
    }
}
