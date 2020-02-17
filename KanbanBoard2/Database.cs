using System;
using System.Threading.Tasks;
using Npgsql;

namespace KanbanBoard2
{
    public class Database
    {
        public static NpgsqlConnection Connect()
        {
            var connection = new NpgsqlConnection("Host=localhost; Port=5432; Username=user; Password=user; Database=KanbanDB;Pooling=False;");
            connection.Open();
            return connection;
        }

        public static NpgsqlDataReader ExecuteCommand(string command)
        {
            var connection = Connect();
            var cmd = new NpgsqlCommand(command, connection);
            var reader = cmd.ExecuteReader();
            reader.Read();

            return reader;
        }
        
        public static async Task SetUpDatabase()
        {
            await CreateTables();
        }

        private static async Task CreateTables()
        {
            var connection = Connect();
            var command = new NpgsqlCommand();
            var transaction = connection.BeginTransaction();
            command.Connection = connection;
            command.Transaction = transaction;
            
            command.CommandText =
                "CREATE TABLE IF NOT EXISTS stories(id SERIAL, name VARCHAR, creator VARCHAR, description VARCHAR)";
            command.ExecuteNonQuery();
            command.CommandText =
                "CREATE TABLE IF NOT EXISTS defects(id SERIAL, name VARCHAR, creator VARCHAR, description VARCHAR, effectedVersion VARCHAR, priority INT)";
            command.ExecuteNonQuery();
            command.CommandText =
                "CREATE TABLE IF NOT EXISTS Tasks(id SERIAL, name VARCHAR, creator VARCHAR, description VARCHAR, subtasks VARCHAR, parent VARCHAR)";
            command.ExecuteNonQuery();
            command.CommandText =
                "CREATE TABLE IF NOT EXISTS boards(id SERIAL, ready VARCHAR, readyWip INT, indevelopment VARCHAR, indevelopmentWip INT, done VARCHAR, doneWip INT)";
            command.ExecuteNonQuery();

            await transaction.CommitAsync();
        }
    }
}