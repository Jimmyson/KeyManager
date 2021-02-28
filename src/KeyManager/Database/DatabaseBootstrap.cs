using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;

namespace KeyManager.Database
{
    class DatabaseBootstrap
    {
        private readonly DatabaseConfig _databaseConfig;

        public DatabaseBootstrap(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public void Setup()
        {
            // Check database exists
            using (var connection = new SqliteConnection(_databaseConfig.ConnectionString))
            {
                // Create KeyRecord Table
                var table = connection.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'KeyRecords';");
                var tableName = table.FirstOrDefault();
                if (!string.IsNullOrEmpty(tableName) && tableName == "KeyRecords")
                    return;
    
                connection.Execute("Create Table KeyRecords (" +
                    "Name VARCHAR(255) NOT NULL," +
                    "Key VARCHAR(255) NOT NULL," +
                    "Note TEXT NULL);");
            }
        }
    }
}