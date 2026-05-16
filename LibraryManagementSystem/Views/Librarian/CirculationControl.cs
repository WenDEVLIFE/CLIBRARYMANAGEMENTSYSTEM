using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Utils;
using System.Collections.Generic;

namespace LibraryManagementSystem.Views.Librarian
{
    public class CirculationControl : UserControl
    {
        private DataGridView dgvBorrows;
        private Button btnNewBorrow;
        private Button btnReturn;
        private Button btnToggleHistory;
        private bool _showHistory = false;
        private BorrowRepository _BorrowRepository;

        public CirculationControl()
        {
            _BorrowRepository = new BorrowRepository();
            InitializeComponent();
            LoadBorrows();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            Panel pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            
            btnNewBorrow = new Button { 
                Text = "+ New Borrow", 
                Width = 120, 
                Height = 35, 
                BackColor = Color.FromArgb(59, 130, 246), 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, 12)
            };
            btnNewBorrow.FlatAppearance.BorderSize = 0;
            btnNewBorrow.Click += BtnNewBorrow_Click;

            btnReturn = new Button { 
                Text = "Mark as Returned", 
                Width = 150, 
                Height = 35, 
                BackColor = Color.FromArgb(16, 185, 129), 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(btnNewBorrow.Right + 10, 12)
            };
            btnReturn.FlatAppearance.BorderSize = 0;
            btnReturn.Click += BtnReturn_Click;

            btnToggleHistory = new Button { 
                Text = "Show All History", 
                Width = 150, 
                Height = 35, 
                BackColor = Color.Gray, 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(btnReturn.Right + 10, 12)
            };
            btnToggleHistory.FlatAppearance.BorderSize = 0;
            btnToggleHistory.Click += (s, e) => {
                _showHistory = !_showHistory;
                btnToggleHistory.Text = _showHistory ? "Show Active Only" : "Show All History";
                LoadBorrows();
            };

            pnlToolbar.Controls.Add(btnNewBorrow);
            pnlToolbar.Controls.Add(btnReturn);
            pnlToolbar.Controls.Add(btnToggleHistory);

            dgvBorrows = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };

            this.Controls.Add(dgvBorrows);
            this.Controls.Add(pnlToolbar);
        }

        private async void LoadBorrows()
        {
            if (_showHistory)
                dgvBorrows.DataSource = await _BorrowRepository.GetAllBorrowsAsync();
            else
                dgvBorrows.DataSource = await _BorrowRepository.GetActiveBorrowsAsync();

            // Configure Columns
            if (dgvBorrows.Columns["BorrowId"] != null) dgvBorrows.Columns["BorrowId"].Visible = false;
            if (dgvBorrows.Columns["BookId"] != null) dgvBorrows.Columns["BookId"].Visible = false;
            if (dgvBorrows.Columns["LibrarianId"] != null) dgvBorrows.Columns["LibrarianId"].Visible = false;
            if (dgvBorrows.Columns["StudentName"] != null) dgvBorrows.Columns["StudentName"].DisplayIndex = 1;
            if (dgvBorrows.Columns["StudentSection"] != null) dgvBorrows.Columns["StudentSection"].DisplayIndex = 2;
            if (dgvBorrows.Columns["StudentSection"] != null) dgvBorrows.Columns["StudentSection"].HeaderText = "Section";
        }

        private async void BtnNewBorrow_Click(object sender, EventArgs e)
        {
            var form = new BorrowForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var Borrow = form.NewBorrow!;
                Borrow.LibrarianId = Session.CurrentUser?.UserId ?? 0;
                
                bool success = await _BorrowRepository.RecordBorrowAsync(Borrow);
                if (success)
                {
                    MessageBox.Show("Borrow recorded successfully.");
                    LoadBorrows();
                }
                else
                {
                    MessageBox.Show("Failed to record Borrow.");
                }
            }
        }

        private async void BtnReturn_Click(object sender, EventArgs e)
        {
            if (dgvBorrows.SelectedRows.Count > 0)
            {
                var Borrow = (Borrow)dgvBorrows.SelectedRows[0].DataBoundItem;
                bool success = await _BorrowRepository.MarkAsReturnedAsync(Borrow.BorrowId, Borrow.BookId);
                if (success)
                {
                    MessageBox.Show("Book returned successfully.");
                    LoadBorrows();
                }
            }
            else
            {
                MessageBox.Show("Please select a Borrow to mark as returned.");
            }
        }
    }
}

