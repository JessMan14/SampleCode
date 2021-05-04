using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SQLService
{
    public interface ISQL_Wrapper
    {
        Task<List<T>> GetItemsAsync<T>(string connectionstring, string storedProcedure);
        Task<List<T>> GetItemsAsync<T>(string connectionstring, string storedProcedure, List<SqlParameter> sqlParameters);
        Task<T> GetItemAsync<T>(string connectionstring, string storedProcedure, List<SqlParameter> sqlParameters);
        Task<List<T>> GetItemsByCommandAsync<T>(string connectionstring, string command);
        Task PerformCommandAsync(string connectionstring, string command);
        Task PerformCommandAsync(string connectionstring, string command, List<SqlParameter> sqlParameters);
        Task<bool> ValidateCommandAsync<T>(string connectionstring, string command);
        Task<T> GetRecord<T>(string connectionString, string commandText);

        Task<long> PerformCommandWithAutoincrementedId(string connectionstring, string command);
        Task<long> PerformCommandWithAutoincrementedId(string connectionstring, string command, List<SqlParameter> sqlParameters);
        Task<List<T>> GetItemsByCommandAsync<T>(string connectionString, string command, List<SqlParameter> parameters);
        Task<DataTable> GetItemsDataTableAsync(string connectionString, string storedProcedure, List<SqlParameter> sqlParameters);
        Task<T> GetItemByCommandAsync<T>(string connectionString, string command, List<SqlParameter> parameters);
        Task PerformSPCommandAsync(string connectionString, string storedProcedure, List<SqlParameter> parameters);
        Task<object> GetGenericItemByCommandAsync(string connectionString, string command, List<SqlParameter> parameters);
        Task<int> PerformSPCommandWithAutoincrementedId(string connectionstring, string storedProcedure, List<SqlParameter> sqlParameters);
    }
}
