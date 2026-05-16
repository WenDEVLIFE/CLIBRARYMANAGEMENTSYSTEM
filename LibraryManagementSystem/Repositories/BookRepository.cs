using MySqlConnector;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class BookRepository
    {
        public async Task<List<Book>> GetAllBooksAsync()
        {
            var books = new List<Book>();
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "SELECT * FROM Books";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                books.Add(new Book
                {
                    BookId = reader.GetInt32("BookId"),
                    Title = reader.GetString("Title"),
                    Author = reader.GetString("Author"),
                    ISBN = reader.IsDBNull(reader.GetOrdinal("ISBN")) ? null : reader.GetString("ISBN"),
                    Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString("Category"),
                    IsAvailable = reader.GetBoolean("IsAvailable"),
                    AddedBy = reader.IsDBNull(reader.GetOrdinal("AddedBy")) ? (int?)null : reader.GetInt32("AddedBy")
                });
            }
            return books;
        }

        public async Task<bool> AddBookAsync(Book book)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "INSERT INTO Books (Title, Author, ISBN, Category, IsAvailable, AddedBy) VALUES (@title, @author, @isbn, @category, @isAvailable, @addedBy)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@title", book.Title);
            command.Parameters.AddWithValue("@author", book.Author);
            command.Parameters.AddWithValue("@isbn", book.ISBN);
            command.Parameters.AddWithValue("@category", book.Category);
            command.Parameters.AddWithValue("@isAvailable", book.IsAvailable);
            command.Parameters.AddWithValue("@addedBy", book.AddedBy);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateAvailabilityAsync(int bookId, bool isAvailable)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "UPDATE Books SET IsAvailable = @isAvailable WHERE BookId = @bookId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@isAvailable", isAvailable);
            command.Parameters.AddWithValue("@bookId", bookId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "UPDATE Books SET Title = @title, Author = @author, ISBN = @isbn, Category = @category WHERE BookId = @bookId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@title", book.Title);
            command.Parameters.AddWithValue("@author", book.Author);
            command.Parameters.AddWithValue("@isbn", book.ISBN);
            command.Parameters.AddWithValue("@category", book.Category);
            command.Parameters.AddWithValue("@bookId", book.BookId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteBookAsync(int bookId)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "DELETE FROM Books WHERE BookId = @bookId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@bookId", bookId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<List<Book>> SearchBooksAsync(string searchTerm)
        {
            var books = new List<Book>();
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "SELECT * FROM Books WHERE Title LIKE @search OR Author LIKE @search OR ISBN LIKE @search";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@search", $"%{searchTerm}%");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                books.Add(new Book
                {
                    BookId = reader.GetInt32("BookId"),
                    Title = reader.GetString("Title"),
                    Author = reader.GetString("Author"),
                    ISBN = reader.IsDBNull(reader.GetOrdinal("ISBN")) ? null : reader.GetString("ISBN"),
                    Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString("Category"),
                    IsAvailable = reader.GetBoolean("IsAvailable"),
                    AddedBy = reader.IsDBNull(reader.GetOrdinal("AddedBy")) ? (int?)null : reader.GetInt32("AddedBy")
                });
            }
            return books;
        }
        public async Task<int> GetBookCountAsync()
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = "SELECT COUNT(*) FROM Books";
            using var command = new MySqlCommand(query, connection);
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }
}
