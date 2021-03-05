using System.Collections.Generic;
using Dapper;
using KeyManager.Models;
using Microsoft.Data.Sqlite;

namespace KeyManager.Database
{
    class KeyRecordRepository
    {
        private readonly DatabaseConfig _databaseConfig;

        public KeyRecordRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }

        public IEnumerable<KeyRecord> GetAll()
        {
            using (var connection = new SqliteConnection(_databaseConfig.ConnectionString))
            {
                return connection.Query<KeyRecord>(
                    "SELECT rowid as Id, Name, Key, Note FROM KeyRecords;");
            }
        }

        public KeyRecord GetSingle(int id)
        {
            using (var connection = new SqliteConnection(_databaseConfig.ConnectionString))
            {
                return connection.QuerySingleOrDefault<KeyRecord>(
                    "SELECT rowid as Id, Name, Key, Note FROM KeyRecords WHERE rowid = @Id;", new {Id = id});
            }
        }

        public void Create(KeyRecord key)
        {
            using (var connection = new SqliteConnection(_databaseConfig.ConnectionString))
            {
                connection.Execute(
                    "INSERT INTO KeyRecords (Name, Key, Note) " +
                    "VALUES (@Name, @Key, @Note);", key);
            }   
        }

        public void Update(KeyRecord key)
        {
            using (var connection = new SqliteConnection(_databaseConfig.ConnectionString))
            {
                connection.Execute(
                    "UPDATE KeyRecords SET " +
                        "Name = @Name, " +
                        "Key = @Key, " +
                        "Note = @Note " +
                    "WHERE rowid = @Id;", key);
            }   
        }

        public void Delete(int id)
        {
            using (var connection = new SqliteConnection(_databaseConfig.ConnectionString))
            {
                connection.Execute(
                    "DELETE FROM KeyRecords " +
                    "WHERE rowid = @Id;", new {Id = id});
            }
        }
    }
}