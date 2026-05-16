using MySqlConnector;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class UserRepository
    {
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "SELECT * FROM Users WHERE Username = @username AND PasswordHash = @password";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password); // Note: Should be hashed in production

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32("UserId"),
                    Username = reader.GetString("Username"),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                    Role = Enum.Parse<UserRole>(reader.GetString("Role")),
                    CreatedAt = reader.GetDateTime("CreatedAt")
                };
            }

            return null;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "INSERT INTO Users (Username, PasswordHash, Email, Role) VALUES (@username, @password, @email, @role)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@password", user.PasswordHash);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@role", user.Role.ToString());

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<int> GetUserCountByRoleAsync(UserRole role)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "SELECT COUNT(*) FROM Users WHERE Role = @role";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@role", role.ToString());

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }
}
