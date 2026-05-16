-- =========================================
-- Library Management System Database Schema
-- Optimized for XAMPP MySQL (MariaDB)
-- =========================================

CREATE DATABASE IF NOT EXISTS LibraryManagementSystemDB;
USE LibraryManagementSystemDB;

-- --------------------------------------------------------
-- Table Structure for 'Users' (Staff Only)
-- Roles: 'Admin', 'Librarian'
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Email VARCHAR(100),
    Role ENUM('Admin', 'Librarian') NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

INSERT IGNORE INTO Users (Username, PasswordHash, Email, Role) 
VALUES ('admin', 'admin123', 'admin@library.com', 'Admin'),
       ('sarah_lib', 'lib123', 'sarah@library.com', 'Librarian');

-- --------------------------------------------------------
-- Table Structure for 'Students' (Dedicated Table)
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS Students (
    StudentId VARCHAR(20) PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Section VARCHAR(10) NOT NULL,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Seed Data: ACT-1B Students
-- Note: Dummy ID numbers provided; replace with actual student IDs
INSERT IGNORE INTO Students (StudentId, FirstName, LastName, Section) VALUES 
('2026-0001', 'Hailey', 'Buenaventura', 'ACT-1B'),
('2026-0002', 'Jeneyah', 'Ariraya', 'ACT-1B'),
('2026-0003', 'Angel', 'Laxama', 'ACT-1B'),
('2026-0004', 'Angela', 'Suba', 'ACT-1B'),
('2026-0005', 'Princess', 'Azada', 'ACT-1B');

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

INSERT IGNORE INTO Books (Title, Author, ISBN, Category, IsAvailable) VALUES 
('The Great Gatsby', 'F. Scott Fitzgerald', '9780743273565', 'Fiction', TRUE),
('Introduction to Algorithms', 'Thomas H. Cormen', '9780262033848', 'Education', TRUE);

-- --------------------------------------------------------
-- Table Structure for 'Loans'
-- Updated to reference the new Students table
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS Loans (
    LoanId INT AUTO_INCREMENT PRIMARY KEY,
    StudentId VARCHAR(20) NOT NULL,
    BookId INT NOT NULL,
    LibrarianId INT NOT NULL,
    BorrowDate DATETIME NOT NULL,
    DueDate DATETIME NOT NULL,
    ReturnDate DATETIME NULL,
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId) ON DELETE CASCADE,
    FOREIGN KEY (BookId) REFERENCES Books(BookId) ON DELETE CASCADE,
    FOREIGN KEY (LibrarianId) REFERENCES Users(UserId) ON DELETE RESTRICT
);

-- --------------------------------------------------------
-- Table Structure for 'Notifications'
-- Updated to reference Students table
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS Notifications (
    NotificationId INT AUTO_INCREMENT PRIMARY KEY,
    StudentId VARCHAR(20) NOT NULL,
    Message TEXT NOT NULL,
    Type ENUM('BorrowConfirmation', 'ReturnReminder') NOT NULL,
    IsRead BOOLEAN DEFAULT FALSE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (StudentId) REFERENCES Students(StudentId) ON DELETE CASCADE
);