using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Views.Admin
{
    public partial class BookForm : Form
    {
        private TextBox txtTitle;
        private TextBox txtAuthor;
        private TextBox txtISBN;
        private TextBox txtCategory;
        private Button btnSave;
        public Book? Book { get; private set; }
        private bool _isEdit;

        public BookForm(Book? book = null)
        {
            Book = book;
            _isEdit = book != null;
            InitializeComponent();
            if (_isEdit && book != null)
            {
                txtTitle.Text = book.Title;
                txtAuthor.Text = book.Author;
                txtISBN.Text = book.ISBN;
                txtCategory.Text = book.Category;
            }
        }

        private void InitializeComponent()
        {
            this.Text = _isEdit ? "Edit Book" : "Add Book";
            this.Size = new Size(400, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            int left = 30, top = 30, width = 320;

            Label lblTitle = new Label { Text = "Title", Location = new Point(left, top), AutoSize = true };
            txtTitle = new TextBox { Location = new Point(left, top + 25), Width = width, Font = new Font("Segoe UI", 10) };

            Label lblAuthor = new Label { Text = "Author", Location = new Point(left, top + 70), AutoSize = true };
            txtAuthor = new TextBox { Location = new Point(left, top + 95), Width = width, Font = new Font("Segoe UI", 10) };

            Label lblISBN = new Label { Text = "ISBN", Location = new Point(left, top + 140), AutoSize = true };
            txtISBN = new TextBox { Location = new Point(left, top + 165), Width = width, Font = new Font("Segoe UI", 10) };

            Label lblCategory = new Label { Text = "Category", Location = new Point(left, top + 210), AutoSize = true };
            txtCategory = new TextBox { Location = new Point(left, top + 235), Width = width, Font = new Font("Segoe UI", 10) };

            btnSave = new Button { 
                Text = "Save Book", 
                Location = new Point(left, top + 300), 
                Width = width, 
                Height = 40, 
                BackColor = Color.FromArgb(59, 130, 246), 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            this.Controls.AddRange(new Control[] { lblTitle, txtTitle, lblAuthor, txtAuthor, lblISBN, txtISBN, lblCategory, txtCategory, btnSave });
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Title is required.");
                return;
            }

            if (_isEdit)
            {
                Book!.Title = txtTitle.Text.Trim();
                Book!.Author = txtAuthor.Text.Trim();
                Book!.ISBN = txtISBN.Text.Trim();
                Book!.Category = txtCategory.Text.Trim();
            }
            else
            {
                Book = new Book
                {
                    Title = txtTitle.Text.Trim(),
                    Author = txtAuthor.Text.Trim(),
                    ISBN = txtISBN.Text.Trim(),
                    Category = txtCategory.Text.Trim(),
                    IsAvailable = true
                };
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
