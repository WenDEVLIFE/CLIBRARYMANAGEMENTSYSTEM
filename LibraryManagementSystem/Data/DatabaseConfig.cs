namespace LibraryManagementSystem.Data
{
    public static class DatabaseConfig
    {
        // Default XAMPP settings
        private const string Server = "localhost";
        private const string Database = "LibraryManagementSystemDB";
        private const string User = "root";
        private const string Password = "";

        public static string ConnectionString => $"Server={Server};Database={Database};User ID={User};Password={Password};SslMode=None;";
    }
}
