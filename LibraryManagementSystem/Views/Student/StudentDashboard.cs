using System;
using System.Windows.Forms;

namespace LibraryManagementSystem.Views.Student
{
    public partial class StudentDashboard : Form
    {
        public StudentDashboard()
        {
            Text = "Student Dashboard";
            Size = new System.Drawing.Size(1024, 768);
            StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(new Label { Text = "Welcome Student!", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Font = new System.Drawing.Font("Segoe UI", 24F) });
        }
    }
}
