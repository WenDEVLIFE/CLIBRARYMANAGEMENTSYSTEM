using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Models;
using System.Collections.Generic;

namespace LibraryManagementSystem.Views.Admin
{
    public class LibrarianManagementControl : UserControl
    {
        private DataGridView dgvLibrarians;
        private Button btnAdd;
        private UserRepository _userRepository;

        public LibrarianManagementControl()
        {
            _userRepository = new UserRepository();
            InitializeComponent();
            LoadLibrarians();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            Panel pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            btnAdd = new Button { 
                Text = "+ Add Librarian", 
                Width = 150, 
                Height = 35, 
                BackColor = Color.FromArgb(59, 130, 246), 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => {
                var form = new LibrarianForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    SaveLibrarian(form.Librarian!);
                }
            };
            pnlToolbar.Controls.Add(btnAdd);

            dgvLibrarians = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            dgvLibrarians.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0) EditLibrarian((User)dgvLibrarians.Rows[e.RowIndex].DataBoundItem);
            };

            this.Controls.Add(dgvLibrarians);
            this.Controls.Add(pnlToolbar);
        }

        private async void LoadLibrarians()
        {
            var librarians = await _userRepository.GetUsersByRoleAsync(UserRole.Librarian);
            dgvLibrarians.DataSource = librarians;
            
            // Hide password hash for security
            if (dgvLibrarians.Columns["PasswordHash"] != null) dgvLibrarians.Columns["PasswordHash"].Visible = false;
        }

        private async void SaveLibrarian(User librarian)
        {
            bool success = await _userRepository.AddUserAsync(librarian);
            if (success)
            {
                MessageBox.Show("Librarian added successfully.");
                LoadLibrarians();
            }
            else
            {
                MessageBox.Show("Failed to add librarian.");
            }
        }

        private async void EditLibrarian(User librarian)
        {
            var form = new LibrarianForm(librarian);
            if (form.ShowDialog() == DialogResult.OK)
            {
                bool success = await _userRepository.UpdateUserAsync(form.Librarian!);
                if (success)
                {
                    MessageBox.Show("Librarian updated successfully.");
                    LoadLibrarians();
                }
            }
        }
    }
}
