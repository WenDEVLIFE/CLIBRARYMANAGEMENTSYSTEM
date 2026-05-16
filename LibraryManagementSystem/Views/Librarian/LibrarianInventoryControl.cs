using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Views.Librarian
{
    public class LibrarianInventoryControl : UserControl
    {
        private DataGridView dgvBooks;
        private TextBox txtSearch;
        private BookRepository _bookRepository;

        public LibrarianInventoryControl()
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
            
            txtSearch = new TextBox { 
                PlaceholderText = "Search books by title, author, or ISBN...", 
                Width = 400, 
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 15)
            };
            txtSearch.TextChanged += async (s, e) => {
                if (string.IsNullOrWhiteSpace(txtSearch.Text)) LoadBooks();
                else dgvBooks.DataSource = await _bookRepository.SearchBooksAsync(txtSearch.Text);
            };

            pnlToolbar.Controls.Add(txtSearch);

            dgvBooks = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9)
            };

            this.Controls.Add(dgvBooks);
            this.Controls.Add(pnlToolbar);
        }

        private async void LoadBooks()
        {
            dgvBooks.DataSource = await _bookRepository.GetAllBooksAsync();
        }
    }
}
