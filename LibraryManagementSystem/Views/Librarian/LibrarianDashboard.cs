using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Views.Librarian
{
    public partial class LibrarianDashboard : Form
    {
        private Panel pnlSidebar;
        private Panel pnlHeader;
        private Panel pnlMainContent;
        private Label lblHeaderTitle;

        public LibrarianDashboard()
        {
            InitializeComponent();
            LoadActivityFeed();
        }

        private void InitializeComponent()
        {
            this.Text = "Scholar Central - Librarian Dashboard";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Theme.Background;

            // Sidebar
            pnlSidebar = new Panel
            {
                Width = 220,
                Dock = DockStyle.Left,
                BackColor = Theme.Secondary
            };

            Label lblLogo = new Label
            {
                Text = "Librarian Portal",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top
            };
            pnlSidebar.Controls.Add(lblLogo);

            AddSidebarButton("Overview", 60, (s, e) => LoadActivityFeed());
            AddSidebarButton("Circulation", 105, (s, e) => LoadCirculation());
            AddSidebarButton("Inventory", 150, (s, e) => LoadInventory());
            
            Button btnLogout = new Button
            {
                Text = "Logout",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Height = 45,
                Dock = DockStyle.Bottom,
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) => { this.Close(); };
            pnlSidebar.Controls.Add(btnLogout);

            // Header
            pnlHeader = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.White
            };

            lblHeaderTitle = new Label
            {
                Text = "Librarian Activity Feed",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            pnlHeader.Controls.Add(lblHeaderTitle);

            // Main Content
            pnlMainContent = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            this.Controls.Add(pnlMainContent);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlSidebar);
        }

        private void AddSidebarButton(string text, int top, EventHandler clickEvent)
        {
            Button btn = new Button
            {
                Text = text,
                Top = top,
                Width = pnlSidebar.Width,
                Height = 45,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(156, 163, 175),
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += clickEvent;
            btn.Click += (s, e) => {
                foreach (Control c in pnlSidebar.Controls) if (c is Button) c.ForeColor = Color.FromArgb(156, 163, 175);
                btn.ForeColor = Color.White;
            };
            pnlSidebar.Controls.Add(btn);
        }

        private async void LoadActivityFeed()
        {
            lblHeaderTitle.Text = "Librarian Activity Feed";
            pnlMainContent.Controls.Clear();

            var BorrowRepo = new BorrowRepository();
            var bookRepo = new BookRepository();

            int activeBorrowsCount = (await BorrowRepo.GetActiveBorrowsAsync()).Count;
            int overdueCount = (await BorrowRepo.GetOverdueBorrowsAsync()).Count;
            int totalBooks = await bookRepo.GetBookCountAsync();

            FlowLayoutPanel flow = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 150 };
            flow.Controls.Add(CreateStatCard("Total Books", totalBooks.ToString(), Color.FromArgb(59, 130, 246)));
            flow.Controls.Add(CreateStatCard("Active Borrows", activeBorrowsCount.ToString(), Color.FromArgb(16, 185, 129)));
            flow.Controls.Add(CreateStatCard("Overdue Books", overdueCount.ToString(), Color.FromArgb(239, 68, 68)));
            pnlMainContent.Controls.Add(flow);

            Label lblOverdue = new Label { Text = "Urgent: Overdue Borrows", Font = new Font("Segoe UI", 12, FontStyle.Bold), Dock = DockStyle.Top, Height = 30, Margin = new Padding(0, 20, 0, 10) };
            pnlMainContent.Controls.Add(lblOverdue);

            DataGridView dgv = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            dgv.DataSource = await BorrowRepo.GetOverdueBorrowsAsync();
            
            if (dgv.Columns["BorrowId"] != null) dgv.Columns["BorrowId"].Visible = false;
            if (dgv.Columns["BookId"] != null) dgv.Columns["BookId"].Visible = false;
            if (dgv.Columns["LibrarianId"] != null) dgv.Columns["LibrarianId"].Visible = false;

            pnlMainContent.Controls.Add(dgv);
        }

        private Panel CreateStatCard(string title, string value, Color color)
        {
            Panel card = new Panel { Width = 250, Height = 100, BackColor = Color.White, Margin = new Padding(0, 0, 20, 0) };
            card.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle, Color.FromArgb(229, 231, 235), ButtonBorderStyle.Solid); };
            Panel stripe = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = color };
            card.Controls.Add(stripe);
            Label lblTitle = new Label { Text = title, Font = new Font("Segoe UI", 10), ForeColor = Color.Gray, Location = new Point(20, 20), AutoSize = true };
            Label lblValue = new Label { Text = value, Font = new Font("Segoe UI", 20, FontStyle.Bold), Location = new Point(20, 45), AutoSize = true };
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);
            return card;
        }

        private void LoadCirculation()
        {
            lblHeaderTitle.Text = "Book Circulation (Borrow/Return)";
            pnlMainContent.Controls.Clear();
            pnlMainContent.Controls.Add(new CirculationControl());
        }

        private void LoadInventory()
        {
            lblHeaderTitle.Text = "Book Inventory View";
            pnlMainContent.Controls.Clear();
            pnlMainContent.Controls.Add(new LibrarianInventoryControl());
        }
    }
}

