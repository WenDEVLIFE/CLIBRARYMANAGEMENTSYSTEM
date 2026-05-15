# Database Implementation Plan

This document outlines the database schema and structure required to support the features defined in the Library Management System.

## 1. Database Schema (Relational)

### Users Table
Stores information for all system users (Admin, Librarian, Student).
- `UserId` (Primary Key, INT, Auto-increment)
- `Username` (VARCHAR, Unique)
- `PasswordHash` (VARCHAR)
- `Email` (VARCHAR)
- `Role` (ENUM: 'Admin', 'Librarian', 'Student')
- `CreatedAt` (DATETIME)

### Books Table
Stores the inventory of books managed by the Admin.
- `BookId` (Primary Key, INT, Auto-increment)
- `Title` (VARCHAR)
- `Author` (VARCHAR)
- `ISBN` (VARCHAR, Unique)
- `Category` (VARCHAR)
- `IsAvailable` (BOOLEAN, Default: TRUE)
- `AddedBy` (Foreign Key -> Users.UserId)

### Loans Table
Managed by the Librarian to track book borrowing.
- `LoanId` (Primary Key, INT, Auto-increment)
- `StudentId` (Foreign Key -> Users.UserId)
- `BookId` (Foreign Key -> Books.BookId)
- `LibrarianId` (Foreign Key -> Users.UserId) - The librarian who logged the loan.
- `BorrowDate` (DATETIME)
- `DueDate` (DATETIME)
- `ReturnDate` (DATETIME, Nullable)

### Notifications Table
Supports the notification feature for Students.
- `NotificationId` (Primary Key, INT, Auto-increment)
- `UserId` (Foreign Key -> Users.UserId)
- `Message` (TEXT)
- `Type` (ENUM: 'BorrowConfirmation', 'ReturnReminder')
- `IsRead` (BOOLEAN, Default: FALSE)
- `CreatedAt` (DATETIME)

## 2. Relationships
- **One-to-Many:** One User (Admin) can add many Books.
- **One-to-Many:** One User (Student) can have many Loans.
- **One-to-Many:** One User (Librarian) can log many Loans.
- **One-to-Many:** One User (Student) can receive many Notifications.

## 3. Data Integrity & Constraints
- **ISBN:** Must be unique for each book.
- **Username:** Must be unique for system access.
- **Foreign Keys:** Cascade deletes should be avoided; use logical deletes or restrict to maintain record history.
- **Availability Toggle:** When a `Loan` is created, the corresponding `Book.IsAvailable` must be set to `FALSE`.

## 4. Implementation Technology
- **Database Engine:** SQLite (for local development) or SQL Server (if integrating with .NET/C# fully).
- **ORM:** Entity Framework Core (recommended for C# integration).
