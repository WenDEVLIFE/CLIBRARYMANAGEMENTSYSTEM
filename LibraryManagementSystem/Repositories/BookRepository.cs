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

            const string query = "INSERT INTO Books (Title, Author, ISBN, Category, AddedBy) VALUES (@title, @author, @isbn, @category, @addedBy)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@title", book.Title);
            command.Parameters.AddWithValue("@author", book.Author);
            command.Parameters.AddWithValue("@isbn", book.ISBN);
            command.Parameters.AddWithValue("@category", book.Category);
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
    }
}
