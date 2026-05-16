using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using System.Collections.Generic;

namespace LibraryManagementSystem.Views.Librarian
{
    public partial class LoanForm : Form
    {
        private ComboBox cbStudents;
        private ComboBox cbBooks;
        private DateTimePicker dtpDueDate;
        private Button btnBorrow;
        private UserRepository _userRepository;
        private BookRepository _bookRepository;

        public Loan? NewLoan { get; private set; }

        public LoanForm()
        {
            _userRepository = new UserRepository();
            _bookRepository = new BookRepository();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Record New Loan";
            this.Size = new Size(400, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            Label lblStudent = new Label { Text = "Select Student", Location = new Point(30, 30), AutoSize = true };
            cbStudents = new ComboBox { Location = new Point(30, 55), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblBook = new Label { Text = "Select Book", Location = new Point(30, 110), AutoSize = true };
            cbBooks = new ComboBox { Location = new Point(30, 135), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblDueDate = new Label { Text = "Due Date", Location = new Point(30, 190), AutoSize = true };
            dtpDueDate = new DateTimePicker { Location = new Point(30, 215), Width = 320, Value = DateTime.Now.AddDays(14) };

            btnBorrow = new Button { 
                Text = "Confirm Loan", 
                Location = new Point(30, 280), 
                Width = 320, 
                Height = 40, 
                BackColor = Color.FromArgb(16, 185, 129), 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnBorrow.FlatAppearance.BorderSize = 0;
            btnBorrow.Click += BtnBorrow_Click;

            this.Controls.AddRange(new Control[] { lblStudent, cbStudents, lblBook, cbBooks, lblDueDate, dtpDueDate, btnBorrow });
        }

        private async void LoadData()
        {
            var students = await _userRepository.GetUsersByRoleAsync(UserRole.Student);
            cbStudents.DataSource = students;
            cbStudents.DisplayMember = "Username";
            cbStudents.ValueMember = "UserId";

            var allBooks = await _bookRepository.GetAllBooksAsync();
            var availableBooks = allBooks.FindAll(b => b.IsAvailable);
            cbBooks.DataSource = availableBooks;
            cbBooks.DisplayMember = "Title";
            cbBooks.ValueMember = "BookId";
        }

        private void BtnBorrow_Click(object sender, EventArgs e)
        {
            if (cbStudents.SelectedValue == null || cbBooks.SelectedValue == null)
            {
                MessageBox.Show("Please select both a student and a book.");
                return;
            }

            NewLoan = new Loan
            {
                StudentId = (int)cbStudents.SelectedValue,
                BookId = (int)cbBooks.SelectedValue,
                BorrowDate = DateTime.Now,
                DueDate = dtpDueDate.Value
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
