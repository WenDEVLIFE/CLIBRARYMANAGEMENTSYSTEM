using MySqlConnector;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class BorrowRepository
    {
        public async Task<bool> RecordBorrowAsync(Borrow borrow)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // 1. Insert Borrow record
                const string borrowQuery = "INSERT INTO Borrows (StudentId, BookId, LibrarianId, BorrowDate, DueDate) VALUES (@studentId, @bookId, @librarianId, @borrowDate, @dueDate)";
                using var borrowCommand = new MySqlCommand(borrowQuery, connection, transaction);
                borrowCommand.Parameters.AddWithValue("@studentId", borrow.StudentId);
                borrowCommand.Parameters.AddWithValue("@bookId", borrow.BookId);
                borrowCommand.Parameters.AddWithValue("@librarianId", borrow.LibrarianId);
                borrowCommand.Parameters.AddWithValue("@borrowDate", borrow.BorrowDate);
                borrowCommand.Parameters.AddWithValue("@dueDate", borrow.DueDate);
                await borrowCommand.ExecuteNonQueryAsync();

                // 2. Update Book availability
                const string bookQuery = "UPDATE Books SET IsAvailable = FALSE WHERE BookId = @bookId";
                using var bookCommand = new MySqlCommand(bookQuery, connection, transaction);
                bookCommand.Parameters.AddWithValue("@bookId", borrow.BookId);
                await bookCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> MarkAsReturnedAsync(int borrowId, int bookId)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                const string borrowQuery = "UPDATE Borrows SET ReturnDate = CURRENT_TIMESTAMP WHERE borrowId = @borrowId";
                using var borrowCommand = new MySqlCommand(borrowQuery, connection, transaction);
                borrowCommand.Parameters.AddWithValue("@borrowId", borrowId);
                await borrowCommand.ExecuteNonQueryAsync();

                const string bookQuery = "UPDATE Books SET IsAvailable = TRUE WHERE BookId = @bookId";
                using var bookCommand = new MySqlCommand(bookQuery, connection, transaction);
                bookCommand.Parameters.AddWithValue("@bookId", bookId);
                await bookCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
        public async Task<List<Borrow>> GetActiveBorrowsAsync()
        {
            var borrows = new List<Borrow>();
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT l.*, CONCAT(s.FirstName, ' ', s.LastName) as StudentName, s.Section as StudentSection, b.Title as BookTitle 
                FROM Borrows l 
                JOIN Students s ON l.StudentId = s.StudentId 
                JOIN Books b ON l.BookId = b.BookId 
                WHERE l.ReturnDate IS NULL";
            
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                borrows.Add(new Borrow
                {
                    BorrowId = reader.GetInt32("BorrowId"),
                    StudentId = reader.GetString("StudentId"),
                    BookId = reader.GetInt32("BookId"),
                    LibrarianId = reader.GetInt32("LibrarianId"),
                    BorrowDate = reader.GetDateTime("BorrowDate"),
                    DueDate = reader.GetDateTime("DueDate"),
                    ReturnDate = reader.IsDBNull(reader.GetOrdinal("ReturnDate")) ? null : reader.GetDateTime("ReturnDate"),
                    StudentName = reader.GetString("StudentName"),
                    StudentSection = reader.GetString("StudentSection"),
                    BookTitle = reader.GetString("BookTitle")
                });
            }
            return borrows;
        }

        public async Task<List<Borrow>> GetAllBorrowsAsync()
        {
            var borrows = new List<Borrow>();
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT l.*, CONCAT(s.FirstName, ' ', s.LastName) as StudentName, s.Section as StudentSection, b.Title as BookTitle 
                FROM Borrows l 
                JOIN Students s ON l.StudentId = s.StudentId 
                JOIN Books b ON l.BookId = b.BookId 
                ORDER BY l.BorrowDate DESC";
            
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                borrows.Add(new Borrow
                {
                    BorrowId = reader.GetInt32("BorrowId"),
                    StudentId = reader.GetString("StudentId"),
                    BookId = reader.GetInt32("BookId"),
                    LibrarianId = reader.GetInt32("LibrarianId"),
                    BorrowDate = reader.GetDateTime("BorrowDate"),
                    DueDate = reader.GetDateTime("DueDate"),
                    ReturnDate = reader.IsDBNull(reader.GetOrdinal("ReturnDate")) ? null : reader.GetDateTime("ReturnDate"),
                    StudentName = reader.GetString("StudentName"),
                    StudentSection = reader.GetString("StudentSection"),
                    BookTitle = reader.GetString("BookTitle")
                });
            }
            return borrows;
        }
        public async Task<List<Borrow>> GetUpcomingDueBorrowsAsync(int daysAhead)
        {
            var borrows = new List<Borrow>();
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT l.*, CONCAT(s.FirstName, ' ', s.LastName) as StudentName, s.Section as StudentSection, b.Title as BookTitle 
                FROM Borrows l 
                JOIN Students s ON l.StudentId = s.StudentId 
                JOIN Books b ON l.BookId = b.BookId 
                WHERE l.ReturnDate IS NULL 
                AND l.DueDate BETWEEN CURRENT_DATE AND DATE_ADD(CURRENT_DATE, INTERVAL @days DAY)";
            
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@days", daysAhead);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                borrows.Add(new Borrow
                {
                    BorrowId = reader.GetInt32("BorrowId"),
                    StudentId = reader.GetString("StudentId"),
                    BookId = reader.GetInt32("BookId"),
                    LibrarianId = reader.GetInt32("LibrarianId"),
                    BorrowDate = reader.GetDateTime("BorrowDate"),
                    DueDate = reader.GetDateTime("DueDate"),
                    ReturnDate = null,
                    StudentName = reader.GetString("StudentName"),
                    StudentSection = reader.GetString("StudentSection"),
                    BookTitle = reader.GetString("BookTitle")
                });
            }
            return borrows;
        }

        public async Task<List<Borrow>> GetOverdueBorrowsAsync()
        {
            var borrows = new List<Borrow>();
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT l.*, CONCAT(s.FirstName, ' ', s.LastName) as StudentName, s.Section as StudentSection, b.Title as BookTitle 
                FROM Borrows l 
                JOIN Students s ON l.StudentId = s.StudentId 
                JOIN Books b ON l.BookId = b.BookId 
                WHERE l.ReturnDate IS NULL AND l.DueDate < CURRENT_DATE";
            
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                borrows.Add(new Borrow
                {
                    BorrowId = reader.GetInt32("BorrowId"),
                    StudentId = reader.GetString("StudentId"),
                    BookId = reader.GetInt32("BookId"),
                    LibrarianId = reader.GetInt32("LibrarianId"),
                    BorrowDate = reader.GetDateTime("BorrowDate"),
                    DueDate = reader.GetDateTime("DueDate"),
                    ReturnDate = null,
                    StudentName = reader.GetString("StudentName"),
                    StudentSection = reader.GetString("StudentSection"),
                    BookTitle = reader.GetString("BookTitle")
                });
            }
            return borrows;
        }
    }
}


