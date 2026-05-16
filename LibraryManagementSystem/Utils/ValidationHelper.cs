using System.Text.RegularExpressions;

namespace LibraryManagementSystem.Utils
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool IsNotEmpty(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
