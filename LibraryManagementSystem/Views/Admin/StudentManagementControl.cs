using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Utils;
using System.Collections.Generic;
using Student = LibraryManagementSystem.Models.Student;

namespace LibraryManagementSystem.Views.Admin
{
    public class StudentManagementControl : UserControl
    {
        private DataGridView dgvStudents;
        private Button btnAdd;
        private StudentRepository _studentRepository;

        public StudentManagementControl()
        {
            _studentRepository = new StudentRepository();
            InitializeComponent();
            LoadStudents();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            Panel pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            btnAdd = new Button { 
                Text = "+ Add Student", 
                Width = 150, 
                Height = 35, 
                BackColor = Color.FromArgb(245, 158, 11), 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => {
                var form = new StudentForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    SaveStudent(form.Student!, form.User!);
                }
            };
            pnlToolbar.Controls.Add(btnAdd);

            dgvStudents = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            dgvStudents.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0) EditStudent((Student)dgvStudents.Rows[e.RowIndex].DataBoundItem);
            };

            this.Controls.Add(dgvStudents);
            this.Controls.Add(pnlToolbar);
        }

        private async void LoadStudents()
        {
            dgvStudents.DataSource = null;
            dgvStudents.DataSource = await _studentRepository.GetAllStudentsAsync();
            if (dgvStudents.Columns["UserId"] != null) dgvStudents.Columns["UserId"].Visible = false;
        }

        private async void SaveStudent(Student student, User user)
        {
            bool success = await _studentRepository.AddStudentAsync(student, user);
            if (success)
            {
                MessageBox.Show("Student and Account created successfully.");
                LoadStudents();
            }
            else
            {
                MessageBox.Show("Failed to add student. Check if Student ID or Username already exists.");
            }
        }

        private async void EditStudent(Student student)
        {
            var form = new StudentForm(student);
            if (form.ShowDialog() == DialogResult.OK)
            {
                bool success = await _studentRepository.UpdateStudentAsync(form.Student!);
                if (success)
                {
                    MessageBox.Show("Student updated successfully.");
                    LoadStudents();
                }
                else
                {
                    MessageBox.Show("Failed to update student.");
                }
            }
        }
    }
}
