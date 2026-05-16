using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Views.Admin
{
    public partial class AdminDashboard : Form
    {
        private Panel pnlSidebar;
        private Panel pnlHeader;
        private Panel pnlMainContent;
        private Label lblHeaderTitle;
        private UserRepository _userRepository;
        private BookRepository _bookRepository;

        public AdminDashboard()
        {
            _userRepository = new UserRepository();
            _bookRepository = new BookRepository();
            InitializeComponent();
            LoadOverview();
        }

        private void InitializeComponent()
        {
            this.Text = "Scholar Central - Admin Dashboard";
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
                Text = "Scholar Central",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top
            };
            pnlSidebar.Controls.Add(lblLogo);

            AddSidebarButton("Overview", 60, (s, e) => LoadOverview());
            AddSidebarButton("Librarians", 105, (s, e) => LoadLibrarianManagement());
            AddSidebarButton("Books", 150, (s, e) => LoadBookManagement());
            
            Button btnLogout = new Button
            {
                Text = "Logout",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = Theme.ButtonFont,
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
                Text = "Dashboard Overview",
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

        private async void LoadOverview()
        {
            lblHeaderTitle.Text = "Dashboard Overview";
            pnlMainContent.Controls.Clear();

            int librarianCount = await _userRepository.GetUserCountByRoleAsync(UserRole.Librarian);
            int studentCount = await _userRepository.GetUserCountByRoleAsync(UserRole.Student);
            var books = await _bookRepository.GetAllBooksAsync();
            int bookCount = books.Count;

            FlowLayoutPanel flow = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 150 };
            flow.Controls.Add(CreateStatCard("Total Books", bookCount.ToString(), Color.FromArgb(59, 130, 246)));
            flow.Controls.Add(CreateStatCard("Librarians", librarianCount.ToString(), Color.FromArgb(16, 185, 129)));
            flow.Controls.Add(CreateStatCard("Students", studentCount.ToString(), Color.FromArgb(245, 158, 11)));

            pnlMainContent.Controls.Add(flow);
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

        private void LoadLibrarianManagement()
        {
            lblHeaderTitle.Text = "Librarian Management";
            pnlMainContent.Controls.Clear();
            pnlMainContent.Controls.Add(new LibrarianManagementControl());
        }

        private void LoadBookManagement()
        {
            lblHeaderTitle.Text = "Book Management";
            pnlMainContent.Controls.Clear();
            pnlMainContent.Controls.Add(new BookManagementControl());
        }
    }
}
