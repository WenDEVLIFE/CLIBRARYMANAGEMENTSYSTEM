using System;
using System.Windows.Forms;

namespace LibraryManagementSystem.Views.Admin
{
    public partial class AdminDashboard : Form
    {
        public AdminDashboard()
        {
            Text = "Admin Dashboard";
            Size = new System.Drawing.Size(1024, 768);
            StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(new Label { Text = "Welcome Admin!", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Font = new System.Drawing.Font("Segoe UI", 24F) });
        }
    }
}
