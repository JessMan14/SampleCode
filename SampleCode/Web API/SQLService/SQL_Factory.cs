using Microsoft.Extensions.Configuration;
using System;

namespace SQLService
{
    public class SQL_Factory
    {
        static SQL_Factory _instance;
        static IConfiguration _config;

        private SQL_Factory() { }

        public static SQL_Factory getInstance(IConfiguration iConfig)
        {
            if (_instance == null)
            {
                _instance = new SQL_Factory();
                _config = iConfig;
            }

            return _instance;
        }

        public ISQL_Wrapper GetWrapper()
        {
            string providerType = (string)_config["SQL_ProviderType"];
            if (providerType == "SQL")
            {
                return new SQL_Wrapper();
            }
            else if (providerType == "MySQL")
            {
                return new MySQL_Wrapper();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public string GetConnectionString()
        {
           string connString = _config.GetConnectionString("Master_Provider");
            if (connString == null)
            {
                throw new Exception("No master provider configured!");
            }
            else return connString;
        }

        public string GetDatabasePrefix()
        {
            string connString = GetConnectionString();
            string[] words = connString.Split(';');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Contains("Initial Catalog"))
                {
                    return words[i].Replace("Initial Catalog=", "");
                }
            }

            return null;
        }

        public string GetDatabasePrefix(int programId)
        {
            string provider = "Program_" + programId.ToString();
            string connectionString = _config.GetConnectionString(provider);
            if (connectionString == null)
            {
                connectionString = GetConnectionString();
            }

            string[] words = connectionString.Split(';');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Contains("Initial Catalog"))
                {
                    return words[i].Replace("Initial Catalog=", "");
                }
            }

            return null;
        }

        public string GetProgramConnectionString()
        {
            return GetConnectionString();
        }

        public string GetProgramConnectionString(int programId)
        {
            string provider = "Program_" + programId.ToString();
            string connectionString = _config.GetConnectionString(provider);
            if (connectionString == null)
            {
                connectionString = GetConnectionString();
            }

            return connectionString;
        }
    }
}