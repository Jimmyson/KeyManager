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
                if (string.IsNullOrEmpty(tableName) || tableName != "KeyRecords")
                {
                    connection.Execute("Create Table IF NOT EXISTS KeyRecords (" +
                        "Name VARCHAR(255) NOT NULL," +
                        "Key VARCHAR(255) NOT NULL," +
                        "Note TEXT NULL);");
                }

                // Create DuplicateKeys View
                table = connection.Query<string>("SELECT name FROM sqlite_master WHERE type='view' AND name = 'DuplicateKeyRecords';");
                tableName = table.FirstOrDefault();
                if (string.IsNullOrEmpty(tableName) || tableName != "DuplicateKeyRecords")
                {
                    connection.Execute("CREATE VIEW IF NOT EXISTS DuplicateKeyRecords AS " +
                        "SELECT rowid as Id, Name, Key, Note FROM KeyRecords where Key IN " +
                        "(SELECT Key FROM KeyRecords Group By Key Having Count(*) > 1) Order By Key, Id;");
                }
            }
        }
    }
}