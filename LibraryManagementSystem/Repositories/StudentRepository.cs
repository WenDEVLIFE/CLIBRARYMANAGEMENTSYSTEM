using MySqlConnector;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using System.Collections.Generic;
using Student = LibraryManagementSystem.Models.Student;

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

        public async Task<bool> AddStudentAsync(Student student, User user)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                const string userQuery = "INSERT INTO Users (Username, PasswordHash, Email, Role) VALUES (@username, @password, @email, 'Student'); SELECT LAST_INSERT_ID();";
                using var userCommand = new MySqlCommand(userQuery, connection, transaction);
                userCommand.Parameters.AddWithValue("@username", user.Username);
                userCommand.Parameters.AddWithValue("@password", user.PasswordHash);
                userCommand.Parameters.AddWithValue("@email", user.Email);
                
                var userId = Convert.ToInt32(await userCommand.ExecuteScalarAsync());

                const string studentQuery = "INSERT INTO Students (StudentId, UserId, FirstName, LastName, Section) VALUES (@studentId, @userId, @firstName, @lastName, @section)";
                using var studentCommand = new MySqlCommand(studentQuery, connection, transaction);
                studentCommand.Parameters.AddWithValue("@studentId", student.StudentId);
                studentCommand.Parameters.AddWithValue("@userId", userId);
                studentCommand.Parameters.AddWithValue("@firstName", student.FirstName);
                studentCommand.Parameters.AddWithValue("@lastName", student.LastName);
                studentCommand.Parameters.AddWithValue("@section", student.Section);

                await studentCommand.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> UpdateStudentAsync(Student student, string? email = null)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                const string studentQuery = "UPDATE Students SET FirstName = @firstName, LastName = @lastName, Section = @section, IsActive = @isActive WHERE StudentId = @studentId";
                using var studentCommand = new MySqlCommand(studentQuery, connection, transaction);
                studentCommand.Parameters.AddWithValue("@firstName", student.FirstName);
                studentCommand.Parameters.AddWithValue("@lastName", student.LastName);
                studentCommand.Parameters.AddWithValue("@section", student.Section);
                studentCommand.Parameters.AddWithValue("@isActive", student.IsActive);
                studentCommand.Parameters.AddWithValue("@studentId", student.StudentId);
                await studentCommand.ExecuteNonQueryAsync();

                if (email != null)
                {
                    const string userQuery = "UPDATE Users SET Email = @email WHERE UserId = @userId";
                    using var userCommand = new MySqlCommand(userQuery, connection, transaction);
                    userCommand.Parameters.AddWithValue("@email", email);
                    userCommand.Parameters.AddWithValue("@userId", student.UserId);
                    await userCommand.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<Student?> GetStudentByUserIdAsync(int userId)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "SELECT * FROM Students WHERE UserId = @userId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Student
                {
                    StudentId = reader.GetString("StudentId"),
                    UserId = reader.GetInt32("UserId"),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    Section = reader.GetString("Section"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedAt = reader.GetDateTime("CreatedAt")
                };
            }
            return null;
        }

        public async Task<bool> DeleteStudentAsync(string studentId, int userId)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // We'll do a hard delete for now as per the schema (ON DELETE CASCADE)
                // But normally we might just set IsActive = false
                const string query = "DELETE FROM Users WHERE UserId = @userId";
                using var command = new MySqlCommand(query, connection, transaction);
                command.Parameters.AddWithValue("@userId", userId);
                
                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
