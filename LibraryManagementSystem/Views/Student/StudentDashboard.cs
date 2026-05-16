using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Utils;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagementSystem.Views.Students
{
    public partial class StudentDashboard : Form
    {
        private Panel pnlSidebar;
        private Panel pnlHeader;
        private Panel pnlMainContent;
        private Label lblHeaderTitle;
        private Button btnNotifications;
        private NotificationRepository _notificationRepository;
        private LoanRepository _loanRepository;
        private StudentRepository _studentRepository;
        private LibraryManagementSystem.Models.Student? _currentStudent;

        public StudentDashboard()
        {
            _notificationRepository = new NotificationRepository();
            _loanRepository = new LoanRepository();
            _studentRepository = new StudentRepository();
            InitializeComponent();
            InitializeDashboard();
        }

        private async void InitializeDashboard()
        {
            if (Session.CurrentUser != null)
            {
                _currentStudent = await _studentRepository.GetStudentByUserIdAsync(Session.CurrentUser.UserId);
            }
            LoadOverview();
            RefreshNotificationCount();
        }

        private void InitializeComponent()
        {
            this.Text = "Scholar Central - Student Dashboard";
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
                Text = "Student Portal",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top
            };
            pnlSidebar.Controls.Add(lblLogo);

            AddSidebarButton("Overview", 60, (s, e) => LoadOverview());
            AddSidebarButton("My Loans", 105, (s, e) => LoadLoans());
            
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
                Text = "Welcome Back!",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 65, 81),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            pnlHeader.Controls.Add(lblHeaderTitle);

            btnNotifications = new Button {
                Text = "🔔 (0)",
                Size = new Size(100, 40),
                Location = new Point(780, 10),
                Anchor = AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnNotifications.FlatAppearance.BorderSize = 0;
            btnNotifications.Click += ShowNotifications;
            pnlHeader.Controls.Add(btnNotifications);

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

        private async void RefreshNotificationCount()
        {
            if (_currentStudent == null) return;
            
            var notes = await _notificationRepository.GetByStudentIdAsync(_currentStudent.StudentId);
            int unread = notes.Count(n => !n.IsRead);
            btnNotifications.Text = $"🔔 ({unread})";
            btnNotifications.ForeColor = unread > 0 ? Color.Red : Color.Black;
        }

        private async void ShowNotifications(object sender, EventArgs e)
        {
            if (_currentStudent == null) return;
            var notes = await _notificationRepository.GetByStudentIdAsync(_currentStudent.StudentId);
            
            Form noteForm = new Form { Text = "Notifications", Size = new Size(300, 400), StartPosition = FormStartPosition.CenterParent };
            ListBox lb = new ListBox { Dock = DockStyle.Fill };
            foreach (var n in notes) lb.Items.Add($"{(n.IsRead ? "" : "* ")}{n.Message}");
            
            lb.DoubleClick += async (s, args) => {
                if (lb.SelectedIndex >= 0) {
                    var selectedNote = notes[lb.SelectedIndex];
                    if (!selectedNote.IsRead) {
                        await _notificationRepository.MarkAsReadAsync(selectedNote.NotificationId);
                        RefreshNotificationCount();
                        noteForm.Close();
                    }
                }
            };

            noteForm.Controls.Add(lb);
            noteForm.ShowDialog();
        }

        private async void LoadOverview()
        {
            lblHeaderTitle.Text = "Student Overview";
            pnlMainContent.Controls.Clear();

            if (_currentStudent == null) return;

            // Run notification service for this student
            var notifyService = new NotificationService();
            await notifyService.ProcessDueNotificationsAsync();
            RefreshNotificationCount();

            Label lblStatus = new Label { 
                Text = $"Welcome, {_currentStudent.FirstName}! Here is your current borrowing status:", 
                Font = new Font("Segoe UI", 12, FontStyle.Bold), 
                Dock = DockStyle.Top, 
                Height = 40 
            };
            pnlMainContent.Controls.Add(lblStatus);

            DataGridView dgv = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            var allLoans = await _loanRepository.GetAllLoansAsync();
            var activeLoans = allLoans.Where(l => l.StudentId == _currentStudent.StudentId && l.ReturnDate == null).ToList();
            dgv.DataSource = activeLoans;

            if (dgv.Columns["LoanId"] != null) dgv.Columns["LoanId"].Visible = false;
            if (dgv.Columns["StudentId"] != null) dgv.Columns["StudentId"].Visible = false;
            if (dgv.Columns["BookId"] != null) dgv.Columns["BookId"].Visible = false;
            if (dgv.Columns["LibrarianId"] != null) dgv.Columns["LibrarianId"].Visible = false;
            if (dgv.Columns["StudentName"] != null) dgv.Columns["StudentName"].Visible = false; // It's them

            pnlMainContent.Controls.Add(dgv);
        }

        private async void LoadLoans()
        {
            lblHeaderTitle.Text = "My Borrowing History";
            pnlMainContent.Controls.Clear();
            
            DataGridView dgv = new DataGridView { 
                Dock = DockStyle.Fill, 
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
 
            var allLoans = await _loanRepository.GetAllLoansAsync();
            var studentLoans = allLoans.Where(l => l.StudentId == _currentStudent?.StudentId).ToList();
            dgv.DataSource = studentLoans;

            // Configure Columns
            if (dgv.Columns["LoanId"] != null) dgv.Columns["LoanId"].Visible = false;
            if (dgv.Columns["StudentId"] != null) dgv.Columns["StudentId"].Visible = false;
            if (dgv.Columns["BookId"] != null) dgv.Columns["BookId"].Visible = false;
            if (dgv.Columns["LibrarianId"] != null) dgv.Columns["LibrarianId"].Visible = false;
            if (dgv.Columns["StudentName"] != null) dgv.Columns["StudentName"].HeaderText = "Name";
            if (dgv.Columns["StudentSection"] != null) dgv.Columns["StudentSection"].HeaderText = "Section";
            if (dgv.Columns["BookTitle"] != null) dgv.Columns["BookTitle"].HeaderText = "Book Title";

            pnlMainContent.Controls.Add(dgv);
        }
    }
}
