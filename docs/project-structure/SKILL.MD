# Project Structure Implementation Plan

This document outlines the recommended folder and architectural structure for the Library Management System project. Since this is a C# Windows Forms Application, we will use a structured approach to separate the User Interface from the Business Logic and Database Access.

## Proposed Folder Structure

```text
LibraryManagementSystem/
│
├── Models/                 # Data entities representing database tables
│   ├── User.cs             # Admin, Librarian, Student
│   ├── Book.cs             # Book details and availability
│   ├── Loan.cs             # Borrowing records
│   └── Notification.cs     # System notifications
│
├── DataAccess/             # Database connection and queries (Repositories)
│   ├── DatabaseContext.cs  # Main database connection setup (e.g., EF Core or SQLite config)
│   ├── UserRepository.cs   # Queries for user authentication and management
│   ├── BookRepository.cs   # Queries for adding/updating books
│   └── LoanRepository.cs   # Queries for borrowing/returning books
│
├── Services/               # Business logic and rules
│   ├── AuthenticationService.cs # Login, role validation, session management
│   ├── BookService.cs           # Logic for book availability
│   └── LoanService.cs           # Logic for borrowing (checks if book is available, creates notification)
│
├── Views/                  # Windows Forms (UI)
│   ├── Auth/
│   │   └── LoginForm.cs         # Login screen
│   ├── Admin/
│   │   ├── AdminDashboard.cs    # Main admin view (stats, add books, create librarians)
│   │   └── ManageBooksForm.cs   # Add/Edit books interface
│   ├── Librarian/
│   │   ├── LibrarianDashboard.cs# Librarian view (inventory, active loans)
│   │   └── LoanManagementForm.cs# Form to log new loans
│   └── Student/
│       └── StudentDashboard.cs  # Student view (notifications, due dates)
│
├── Utilities/              # Helper classes
│   ├── PasswordHasher.cs   # Security utility for hashing passwords
│   └── SessionManager.cs   # Tracks the currently logged-in user
│
└── Program.cs              # Application entry point
```

## Architectural Principles

1. **Separation of Concerns:** Forms (`Views/`) should not contain direct SQL queries. All database interactions should happen through the `DataAccess/` layer.
2. **Business Logic Layer:** The `Services/` folder handles the rules. For example, when a Librarian logs a loan in `LoanManagementForm`, the form calls `LoanService.LogLoan()`. The service checks if the book is available, saves the loan via `LoanRepository`, and updates the book's status via `BookRepository`.
3. **Scalability:** By keeping the UI separate from the database code, it becomes much easier to maintain, test, and update the application later.
