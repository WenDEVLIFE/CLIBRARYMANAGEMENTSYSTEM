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
    }
}
