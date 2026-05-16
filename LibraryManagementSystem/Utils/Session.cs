using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Utils
{
    public static class Session
    {
        public static User? CurrentUser { get; set; }
    }
}
