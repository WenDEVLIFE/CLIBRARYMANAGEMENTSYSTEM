using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Models;
using System.Collections.Generic;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Views.Admin
{
    public class AdminManagementControl : UserControl
    {
        private DataGridView dgvAdmins;
        private Button btnAdd;
        private UserRepository _userRepository;

        public AdminManagementControl()
        {
            _userRepository = new UserRepository();
            InitializeComponent();
            LoadAdmins();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            Panel pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            btnAdd = new Button { 
                Text = "+ Add Admin", 
                Width = 150, 
                Height = 35, 
                BackColor = Color.FromArgb(59, 130, 246), 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => {
                var form = new UserForm(UserRole.Admin);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    SaveAdmin(form.User!);
                }
            };
            pnlToolbar.Controls.Add(btnAdd);

            dgvAdmins = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            dgvAdmins.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0) EditAdmin((User)dgvAdmins.Rows[e.RowIndex].DataBoundItem);
            };

            this.Controls.Add(dgvAdmins);
            this.Controls.Add(pnlToolbar);
        }

        private async void LoadAdmins()
        {
            var admins = await _userRepository.GetUsersByRoleAsync(UserRole.Admin);
            dgvAdmins.DataSource = admins;
            
            if (dgvAdmins.Columns["PasswordHash"] != null) dgvAdmins.Columns["PasswordHash"].Visible = false;
        }

        private async void SaveAdmin(User admin)
        {
            bool success = await _userRepository.AddUserAsync(admin);
            if (success)
            {
                MessageBox.Show("Admin added successfully.");
                LoadAdmins();
            }
            else
            {
                MessageBox.Show("Failed to add admin.");
            }
        }

        private async void EditAdmin(User admin)
        {
            var form = new UserForm(UserRole.Admin, admin);
            if (form.ShowDialog() == DialogResult.OK)
            {
                bool success = await _userRepository.UpdateUserAsync(form.User!);
                if (success)
                {
                    MessageBox.Show("Admin updated successfully.");
                    LoadAdmins();
                }
            }
        }
    }
}
