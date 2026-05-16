using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Views.Admin
{
    public partial class LibrarianForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnSave;
        private Label lblPassword;
        public User? Librarian { get; private set; }
        private bool _isEdit;

        public LibrarianForm(User? librarian = null)
        {
            Librarian = librarian;
            _isEdit = librarian != null;
            InitializeComponent();
            if (_isEdit && librarian != null)
            {
                txtUsername.Text = librarian.Username;
                txtEmail.Text = librarian.Email;
                txtPassword.Visible = false;
                lblPassword.Visible = false;
                this.Height -= 60;
                btnSave.Top -= 60;
            }
        }

        private void InitializeComponent()
        {
            this.Text = _isEdit ? "Edit Librarian" : "Add Librarian";
            this.Size = new Size(400, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            Label lblUsername = new Label { Text = "Username", Location = new Point(30, 30), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(30, 55), Width = 320, Font = new Font("Segoe UI", 10) };

            Label lblEmail = new Label { Text = "Email", Location = new Point(30, 100), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(30, 125), Width = 320, Font = new Font("Segoe UI", 10) };

            lblPassword = new Label { Text = "Password", Location = new Point(30, 170), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(30, 195), Width = 320, Font = new Font("Segoe UI", 10), PasswordChar = '*' };

            btnSave = new Button { 
                Text = "Save Librarian", 
                Location = new Point(30, 260), 
                Width = 320, 
                Height = 40, 
                BackColor = Color.FromArgb(16, 185, 129), 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            this.Controls.AddRange(new Control[] { lblUsername, txtUsername, lblEmail, txtEmail, lblPassword, txtPassword, btnSave });
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidationHelper.IsNotEmpty(txtUsername.Text))
            {
                MessageBox.Show("Username is required.");
                return;
            }

            if (ValidationHelper.IsNotEmpty(txtEmail.Text) && !ValidationHelper.IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address.");
                return;
            }

            if (!_isEdit && !ValidationHelper.IsNotEmpty(txtPassword.Text))
            {
                MessageBox.Show("Password is required for new accounts.");
                return;
            }

            if (_isEdit)
            {
                Librarian!.Username = txtUsername.Text.Trim();
                Librarian!.Email = txtEmail.Text.Trim();
            }
            else
            {
                Librarian = new User
                {
                    Username = txtUsername.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    PasswordHash = PasswordHasher.HashPassword(txtPassword.Text.Trim()),
                    Role = UserRole.Librarian
                };
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
