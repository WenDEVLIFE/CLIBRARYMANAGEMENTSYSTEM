using MySqlConnector;
using System.Data;

namespace LibraryManagementSystem.Data
{
    public static class DatabaseHelper
    {
        /// <summary>
        /// Gets a new, unopened connection to the database.
        /// </summary>
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(DatabaseConfig.ConnectionString);
        }

        /// <summary>
        /// Tests the database connection.
        /// </summary>
        public static async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var connection = GetConnection();
                await connection.OpenAsync();
                return connection.State == ConnectionState.Open;
            }
            catch (Exception ex)
            {
                // In a real app, log this error
                Console.WriteLine($"Database connection failed: {ex.Message}");
                return false;
            }
        }
    }
}
