-- =========================================
-- Library Management System Database Schema
-- Optimized for XAMPP MySQL (MariaDB)
-- =========================================

-- Create the database if it doesn't exist and use it
CREATE DATABASE IF NOT EXISTS LibraryManagementSystemDB;
USE LibraryManagementSystemDB;

-- --------------------------------------------------------
-- Table Structure for 'Users'
-- Roles: 'Admin', 'Librarian', 'Student'
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Email VARCHAR(100),
    Role ENUM('Admin', 'Librarian', 'Student') NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Insert a default Admin account (Password should be hashed in production!)
-- Assuming password is 'admin123'
INSERT IGNORE INTO Users (Username, PasswordHash, Email, Role) 
VALUES ('admin', 'admin123', 'admin@library.com', 'Admin');

-- --------------------------------------------------------
-- Table Structure for 'Books'
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS Books (
    BookId INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Author VARCHAR(150) NOT NULL,
    ISBN VARCHAR(20) UNIQUE,
    Category VARCHAR(100),
    IsAvailable BOOLEAN DEFAULT TRUE,
    AddedBy INT,
    FOREIGN KEY (AddedBy) REFERENCES Users(UserId) ON DELETE SET NULL
);

-- --------------------------------------------------------
-- Table Structure for 'Loans'
-- Tracks book borrowing
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS Loans (
    LoanId INT AUTO_INCREMENT PRIMARY KEY,
    StudentId INT NOT NULL,
    BookId INT NOT NULL,
    LibrarianId INT NOT NULL,
    BorrowDate DATETIME NOT NULL,
    DueDate DATETIME NOT NULL,
    ReturnDate DATETIME NULL,
    FOREIGN KEY (StudentId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (BookId) REFERENCES Books(BookId) ON DELETE CASCADE,
    FOREIGN KEY (LibrarianId) REFERENCES Users(UserId) ON DELETE RESTRICT
);

-- --------------------------------------------------------
-- Table Structure for 'Notifications'
-- System reminders for students
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS Notifications (
    NotificationId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    Message TEXT NOT NULL,
    Type ENUM('BorrowConfirmation', 'ReturnReminder') NOT NULL,
    IsRead BOOLEAN DEFAULT FALSE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);
