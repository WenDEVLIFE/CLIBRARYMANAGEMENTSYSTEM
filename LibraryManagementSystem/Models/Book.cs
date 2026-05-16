namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public string? Category { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int? AddedBy { get; set; }
        public string? AddedByUsername { get; set; }
    }
}
