using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Models;
using System.Collections.Generic;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Views.Admin
{
    public class BookManagementControl : UserControl
    {
        private DataGridView dgvBooks;
        private Button btnAdd;
        private TextBox txtSearch;
        private BookRepository _bookRepository;

        public BookManagementControl()
        {
            _bookRepository = new BookRepository();
            InitializeComponent();
            LoadBooks();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            Panel pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            
            btnAdd = new Button { 
                Text = "+ Add Book", 
                Width = 120, 
                Height = 35, 
                BackColor = Color.FromArgb(59, 130, 246), 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, 12)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => {
                var form = new BookForm();
                if (form.ShowDialog() == DialogResult.OK) SaveBook(form.Book!);
            };

            txtSearch = new TextBox { 
                PlaceholderText = "Search books...", 
                Width = 250, 
                Font = new Font("Segoe UI", 10),
                Location = new Point(btnAdd.Right + 20, 15)
            };
            txtSearch.TextChanged += async (s, e) => {
                if (string.IsNullOrWhiteSpace(txtSearch.Text)) LoadBooks();
                else dgvBooks.DataSource = await _bookRepository.SearchBooksAsync(txtSearch.Text);
            };

            pnlToolbar.Controls.Add(btnAdd);
            pnlToolbar.Controls.Add(txtSearch);

            dgvBooks = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };
            dgvBooks.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0) EditBook((Book)dgvBooks.Rows[e.RowIndex].DataBoundItem);
            };

            this.Controls.Add(dgvBooks);
            this.Controls.Add(pnlToolbar);
        }

        private async void LoadBooks()
        {
            dgvBooks.DataSource = await _bookRepository.GetAllBooksAsync();
        }

        private async void SaveBook(Book book)
        {
            bool success = await _bookRepository.AddBookAsync(book);
            if (success)
            {
                MessageBox.Show("Book added successfully.");
                LoadBooks();
            }
        }

        private async void EditBook(Book book)
        {
            var form = new BookForm(book);
            if (form.ShowDialog() == DialogResult.OK)
            {
                bool success = await _bookRepository.UpdateBookAsync(form.Book!);
                if (success)
                {
                    MessageBox.Show("Book updated successfully.");
                    LoadBooks();
                }
            }
        }
    }
}
