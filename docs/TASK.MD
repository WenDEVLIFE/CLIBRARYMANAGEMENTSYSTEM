# Library Management System - Detailed Task List

## 1. Foundation & Database Layer
- `[x]` Database Schema Design (SCHEMA.SQL)
- `[x]` Database Connection Helper
    - `[x]` Implement `DatabaseConfig` for connection strings
    - `[x]` Create `MySqlContext` / `DatabaseHelper` class
- `[ ]` Base Repository / Service Pattern
    - `[ ]` Create `UserRepository` for auth and user management
    - `[ ]` Create `BookRepository` for inventory management
    - `[ ]` Create `LoanRepository` for borrowing records
    - `[ ]` Create `NotificationRepository` for system alerts

## 2. Authentication Module
- `[x]` Login Form UI (LoginForm.cs)
- `[ ]` Login Backend Logic
    - `[ ]` Password Hashing/Verification function
    - `[ ]` Database query for user authentication
    - `[ ]` Session/Role handling (determining which dashboard to open)

## 3. Admin Module
- `[ ]` Admin Dashboard UI
    - `[ ]` Navigation Sidebar & Header
    - `[ ]` Stats Cards (Total Books, Librarians, Students)
- `[ ]` Librarian Management
    - `[ ]` **UI**: Librarian List View & Registration Form
    - `[ ]` **DB**: `CreateLibrarian`, `UpdateLibrarian`, `DeleteLibrarian` functions
- `[ ]` Book Management
    - `[ ]` **UI**: Book List View & Add/Edit Book Dialog
    - `[ ]` **DB**: `AddBook`, `EditBook`, `DeleteBook` functions

## 4. Librarian Module
- `[ ]` Librarian Dashboard UI
    - `[ ]` Main activity feed
- `[ ]` Book Inventory View
    - `[ ]` **UI**: Searchable book grid
    - `[ ]` **DB**: `GetAllBooks`, `SearchBooks` functions
- `[ ]` Loan Operations
    - `[ ]` **UI**: Borrowing Record Form (Select Student, Select Book, Set Due Date)
    - `[ ]` **UI**: Return Book Interface
    - `[ ]` **DB**: `RecordLoan`, `MarkAsReturned`, `UpdateBookAvailability` functions

## 5. Student Module
- `[ ]` Student Dashboard UI
    - `[ ]` Overview of currently borrowed books
- `[ ]` Notification System
    - `[ ]` **UI**: Notification Bell & Popup List
    - `[ ]` **DB**: `GetNotificationsByUserId`, `MarkNotificationAsRead` functions
- `[ ]` Borrowing History
    - `[ ]` **UI**: Grid showing past and current loans

## 6. UI/UX & Finalization
- `[ ]` Theme Implementation
    - `[ ]` Apply custom colors and fonts to all WinForms
    - `[ ]` Implement hover effects and button styling
- `[ ]` Validation & Security
    - `[ ]` Form input validation (Regex for emails, required fields)
    - `[ ]` Error handling for database failures
- `[ ]` Final Testing
    - `[ ]` End-to-end testing for all roles
