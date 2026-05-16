using MySqlConnector;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories
{
    public class LoanRepository
    {
        public async Task<bool> RecordLoanAsync(Loan loan)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // 1. Insert Loan record
                const string loanQuery = "INSERT INTO Loans (StudentId, BookId, LibrarianId, BorrowDate, DueDate) VALUES (@studentId, @bookId, @librarianId, @borrowDate, @dueDate)";
                using var loanCommand = new MySqlCommand(loanQuery, connection, transaction);
                loanCommand.Parameters.AddWithValue("@studentId", loan.StudentId);
                loanCommand.Parameters.AddWithValue("@bookId", loan.BookId);
                loanCommand.Parameters.AddWithValue("@librarianId", loan.LibrarianId);
                loanCommand.Parameters.AddWithValue("@borrowDate", loan.BorrowDate);
                loanCommand.Parameters.AddWithValue("@dueDate", loan.DueDate);
                await loanCommand.ExecuteNonQueryAsync();

                // 2. Update Book availability
                const string bookQuery = "UPDATE Books SET IsAvailable = FALSE WHERE BookId = @bookId";
                using var bookCommand = new MySqlCommand(bookQuery, connection, transaction);
                bookCommand.Parameters.AddWithValue("@bookId", loan.BookId);
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

        public async Task<bool> MarkAsReturnedAsync(int loanId, int bookId)
        {
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                const string loanQuery = "UPDATE Loans SET ReturnDate = CURRENT_TIMESTAMP WHERE LoanId = @loanId";
                using var loanCommand = new MySqlCommand(loanQuery, connection, transaction);
                loanCommand.Parameters.AddWithValue("@loanId", loanId);
                await loanCommand.ExecuteNonQueryAsync();

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
        public async Task<List<Loan>> GetActiveLoansAsync()
        {
            var loans = new List<Loan>();
            using var connection = DatabaseHelper.GetConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT l.*, CONCAT(s.FirstName, ' ', s.LastName) as StudentName, b.Title as BookTitle 
                FROM Loans l 
                JOIN Students s ON l.StudentId = s.StudentId 
                JOIN Books b ON l.BookId = b.BookId 
                WHERE l.ReturnDate IS NULL";
            
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                loans.Add(new Loan
                {
                    LoanId = reader.GetInt32("LoanId"),
                    StudentId = reader.GetString("StudentId"),
                    BookId = reader.GetInt32("BookId"),
                    LibrarianId = reader.GetInt32("LibrarianId"),
                    BorrowDate = reader.GetDateTime("BorrowDate"),
                    DueDate = reader.GetDateTime("DueDate"),
                    ReturnDate = reader.IsDBNull(reader.GetOrdinal("ReturnDate")) ? null : reader.GetDateTime("ReturnDate"),
                    StudentName = reader.GetString("StudentName"),
                    BookTitle = reader.GetString("BookTitle")
                });
            }
            return loans;
        }
    }
}
