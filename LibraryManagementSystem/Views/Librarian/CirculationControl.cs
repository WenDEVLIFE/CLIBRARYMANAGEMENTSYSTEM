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
        private DataGridView dgvLoans;
        private Button btnNewLoan;
        private Button btnReturn;
        private LoanRepository _loanRepository;

        public CirculationControl()
        {
            _loanRepository = new LoanRepository();
            InitializeComponent();
            LoadLoans();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            Panel pnlToolbar = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            
            btnNewLoan = new Button { 
                Text = "+ New Loan", 
                Width = 120, 
                Height = 35, 
                BackColor = Color.FromArgb(59, 130, 246), 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(10, 12)
            };
            btnNewLoan.FlatAppearance.BorderSize = 0;
            btnNewLoan.Click += BtnNewLoan_Click;

            btnReturn = new Button { 
                Text = "Mark as Returned", 
                Width = 150, 
                Height = 35, 
                BackColor = Color.FromArgb(16, 185, 129), 
                ForeColor = Color.White, 
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(btnNewLoan.Right + 10, 12)
            };
            btnReturn.FlatAppearance.BorderSize = 0;
            btnReturn.Click += BtnReturn_Click;

            pnlToolbar.Controls.Add(btnNewLoan);
            pnlToolbar.Controls.Add(btnReturn);

            dgvLoans = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };

            this.Controls.Add(dgvLoans);
            this.Controls.Add(pnlToolbar);
        }

        private async void LoadLoans()
        {
            dgvLoans.DataSource = await _loanRepository.GetActiveLoansAsync();
        }

        private async void BtnNewLoan_Click(object sender, EventArgs e)
        {
            var form = new LoanForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var loan = form.NewLoan!;
                loan.LibrarianId = Session.CurrentUser?.UserId ?? 0;
                
                bool success = await _loanRepository.RecordLoanAsync(loan);
                if (success)
                {
                    MessageBox.Show("Loan recorded successfully.");
                    LoadLoans();
                }
                else
                {
                    MessageBox.Show("Failed to record loan.");
                }
            }
        }

        private async void BtnReturn_Click(object sender, EventArgs e)
        {
            if (dgvLoans.SelectedRows.Count > 0)
            {
                var loan = (Loan)dgvLoans.SelectedRows[0].DataBoundItem;
                bool success = await _loanRepository.MarkAsReturnedAsync(loan.LoanId, loan.BookId);
                if (success)
                {
                    MessageBox.Show("Book returned successfully.");
                    LoadLoans();
                }
            }
            else
            {
                MessageBox.Show("Please select a loan to mark as returned.");
            }
        }
    }
}
