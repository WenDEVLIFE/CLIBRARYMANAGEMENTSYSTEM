using System;
using System.Windows.Forms;

namespace LibraryManagementSystem.Views.Librarian
{
    public partial class LibrarianDashboard : Form
    {
        public LibrarianDashboard()
        {
            Text = "Librarian Dashboard";
            Size = new System.Drawing.Size(1024, 768);
            StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(new Label { Text = "Welcome Librarian!", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Font = new System.Drawing.Font("Segoe UI", 24F) });
        }
    }
}
