namespace LibraryManagementSystem.Models
{
    public class Loan
    {
        public int LoanId { get; set; }
        public int StudentId { get; set; }
        public int BookId { get; set; }
        public int LibrarianId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        // Navigation properties (optional for now, but helpful)
        public string? StudentName { get; set; }
        public string? BookTitle { get; set; }
    }
}
