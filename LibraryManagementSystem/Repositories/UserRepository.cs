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

            const string query = "SELECT * FROM Users WHERE Username = @username";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                string storedHash = reader.GetString("PasswordHash");
                
                // Verify password (in production, even the default admin should be hashed)
                // For convenience during initial setup, we'll check plain text if it's not a BCrypt hash
                bool isValid = storedHash.StartsWith("$2") 
                    ? Utils.PasswordHasher.VerifyPassword(password, storedHash)
                    : storedHash == password;

                if (isValid)
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

        public async Task<bool> UpdateUserAsync(User user)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "UPDATE Users SET Username = @username, Email = @email, Role = @role WHERE UserId = @userId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@role", user.Role.ToString());
            command.Parameters.AddWithValue("@userId", user.UserId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "DELETE FROM Users WHERE UserId = @userId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<List<User>> GetUsersByRoleAsync(UserRole role)
        {
            var users = new List<User>();
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "SELECT * FROM Users WHERE Role = @role";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@role", role.ToString());

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    UserId = reader.GetInt32("UserId"),
                    Username = reader.GetString("Username"),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                    Role = Enum.Parse<UserRole>(reader.GetString("Role")),
                    CreatedAt = reader.GetDateTime("CreatedAt")
                });
            }
            return users;
        }
    }
}
