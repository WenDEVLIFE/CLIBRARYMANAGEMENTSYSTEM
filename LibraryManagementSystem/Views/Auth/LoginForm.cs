using System;
using System.Windows.Forms;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Views.Admin;
using LibraryManagementSystem.Views.Librarian;
using LibraryManagementSystem.Views.Student;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Views.Auth
{
    public partial class LoginForm : Form
    {
        private readonly UserRepository _userRepository;

        public LoginForm()
        {
            InitializeComponent();
            _userRepository = new UserRepository();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnLogin.Enabled = false;
                btnLogin.Text = "Logging in...";

                User? user = await _userRepository.AuthenticateAsync(username, password);

                if (user != null)
                {
                    Session.CurrentUser = user;
                    this.Hide();
                    Form dashboard = user.Role switch
                    {
                        UserRole.Admin => new AdminDashboard(),
                        UserRole.Librarian => new LibrarianDashboard(),
                        UserRole.Student => new StudentDashboard(),
                        _ => throw new Exception("Invalid user role.")
                    };

                    dashboard.Show();
                    dashboard.FormClosed += (s, args) => this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnLogin.Enabled = true;
                    btnLogin.Text = "Login";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during login: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
            }
        }
    }
}
