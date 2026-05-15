using System;
using System.Windows.Forms;

namespace LibraryManagementSystem.Views.Auth
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Login functionality will be implemented soon.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
