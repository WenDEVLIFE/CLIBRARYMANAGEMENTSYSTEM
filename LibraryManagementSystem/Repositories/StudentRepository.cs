using MySqlConnector;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using System.Collections.Generic;

namespace LibraryManagementSystem.Repositories
{
    public class StudentRepository
    {
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            var students = new List<Student>();
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "SELECT * FROM Students";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                students.Add(new Student
                {
                    StudentId = reader.GetString("StudentId"),
                    UserId = reader.GetInt32("UserId"),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    Section = reader.GetString("Section"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedAt = reader.GetDateTime("CreatedAt")
                });
            }
            return students;
        }

        public async Task<int> GetStudentCountAsync()
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "SELECT COUNT(*) FROM Students WHERE IsActive = TRUE";
            using var command = new MySqlCommand(query, connection);
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }
}
