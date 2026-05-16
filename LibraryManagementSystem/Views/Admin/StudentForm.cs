using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Utils;
using Student = LibraryManagementSystem.Models.Student;

namespace LibraryManagementSystem.Views.Admin
{
    public partial class StudentForm : Form
    {
        private TextBox txtStudentId;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private TextBox txtSection;
        private TextBox txtUsername;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnSave;
        
        private Label lblUsername;
        private Label lblPassword;

        public Student? Student { get; private set; }
        public User? User { get; private set; }
        private bool _isEdit;

        public StudentForm(Student? student = null)
        {
            Student = student;
            _isEdit = student != null;
            InitializeComponent();
            
            if (_isEdit && student != null)
            {
                txtStudentId.Text = student.StudentId;
                txtStudentId.ReadOnly = true;
                txtFirstName.Text = student.FirstName;
                txtLastName.Text = student.LastName;
                txtSection.Text = student.Section;
                
                // Account section usually not edited here for simplicity, 
                // but we could load the user email if needed.
                // For this implementation, we'll hide account fields on edit
                txtUsername.Visible = false;
                lblUsername.Visible = false;
                txtPassword.Visible = false;
                lblPassword.Visible = false;
                this.Height -= 120;
                btnSave.Top -= 120;
            }
        }

        private void InitializeComponent()
        {
            this.Text = _isEdit ? "Edit Student" : "Add Student";
            this.Size = new Size(450, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            int left = 30;
            int width = 370;

            Label lblStudentId = new Label { Text = "Student ID", Location = new Point(left, 20), AutoSize = true };
            txtStudentId = new TextBox { Location = new Point(left, 45), Width = width, Font = new Font("Segoe UI", 10) };

            Label lblFirstName = new Label { Text = "First Name", Location = new Point(left, 85), AutoSize = true };
            txtFirstName = new TextBox { Location = new Point(left, 110), Width = width, Font = new Font("Segoe UI", 10) };

            Label lblLastName = new Label { Text = "Last Name", Location = new Point(left, 150), AutoSize = true };
            txtLastName = new TextBox { Location = new Point(left, 175), Width = width, Font = new Font("Segoe UI", 10) };

            Label lblSection = new Label { Text = "Section (e.g. ACT-1B)", Location = new Point(left, 215), AutoSize = true };
            txtSection = new TextBox { Location = new Point(left, 240), Width = width, Font = new Font("Segoe UI", 10) };

            lblUsername = new Label { Text = "Account Username (Used for Login)", Location = new Point(left, 280), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(left, 305), Width = width, Font = new Font("Segoe UI", 10) };

            Label lblEmail = new Label { Text = "Email (Optional)", Location = new Point(left, 345), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(left, 370), Width = width, Font = new Font("Segoe UI", 10) };

            lblPassword = new Label { Text = "Password", Location = new Point(left, 410), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(left, 435), Width = width, Font = new Font("Segoe UI", 10), PasswordChar = '*' };

            btnSave = new Button { 
                Text = "Save Student", 
                Location = new Point(left, 500), 
                Width = width, 
                Height = 45, 
                BackColor = Color.FromArgb(245, 158, 11), 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            this.Controls.AddRange(new Control[] { 
                lblStudentId, txtStudentId, 
                lblFirstName, txtFirstName, 
                lblLastName, txtLastName, 
                lblSection, txtSection,
                lblUsername, txtUsername,
                lblEmail, txtEmail,
                lblPassword, txtPassword,
                btnSave 
            });
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentId.Text) || 
                string.IsNullOrWhiteSpace(txtFirstName.Text) || 
                string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Student ID, First Name, and Last Name are required.");
                return;
            }

            if (!_isEdit)
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Username and Password are required for new accounts.");
                    return;
                }

                User = new User
                {
                    Username = txtUsername.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    PasswordHash = PasswordHasher.HashPassword(txtPassword.Text.Trim()),
                    Role = UserRole.Student
                };

                Student = new Student
                {
                    StudentId = txtStudentId.Text.Trim(),
                    FirstName = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    Section = txtSection.Text.Trim()
                };
            }
            else
            {
                Student!.FirstName = txtFirstName.Text.Trim();
                Student!.LastName = txtLastName.Text.Trim();
                Student!.Section = txtSection.Text.Trim();
                // Optionally handle email update if we added an email field for edit
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
