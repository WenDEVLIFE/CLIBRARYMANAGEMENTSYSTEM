using MySqlConnector;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class NotificationRepository
    {
        public async Task<bool> AddNotificationAsync(Notification notification)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "INSERT INTO Notifications (StudentId, Message, Type) VALUES (@studentId, @message, @type)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@studentId", notification.StudentId);
            command.Parameters.AddWithValue("@message", notification.Message);
            command.Parameters.AddWithValue("@type", notification.Type.ToString());

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<List<Notification>> GetByStudentIdAsync(string studentId)
        {
            var notifications = new List<Notification>();
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "SELECT * FROM Notifications WHERE StudentId = @studentId ORDER BY CreatedAt DESC";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@studentId", studentId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                notifications.Add(new Notification
                {
                    NotificationId = reader.GetInt32("NotificationId"),
                    StudentId = reader.GetString("StudentId"),
                    Message = reader.GetString("Message"),
                    Type = Enum.Parse<NotificationType>(reader.GetString("Type")),
                    IsRead = reader.GetBoolean("IsRead"),
                    CreatedAt = reader.GetDateTime("CreatedAt")
                });
            }
            return notifications;
        }
        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "UPDATE Notifications SET IsRead = TRUE WHERE NotificationId = @id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", notificationId);

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
}
