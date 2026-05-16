-- =========================================
-- Library Management System Database Schema
-- Optimized for XAMPP MySQL (MariaDB)
-- =========================================

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

INSERT IGNORE INTO Users (UserId, Username, PasswordHash, Email, Role) 
VALUES (1, 'admin', 'admin123', 'admin@library.com', 'Admin'),
       (2, 'sarah_lib', 'lib123', 'sarah@library.com', 'Librarian'),
       (3, '2026-0001', 'student123', 'hailey@student.com', 'Student'),
       (4, '2026-0002', 'student123', 'jeneyah@student.com', 'Student'),
       (5, '2026-0003', 'student123', 'angel@student.com', 'Student'),
       (6, '2026-0004', 'student123', 'angela@student.com', 'Student'),
       (7, '2026-0005', 'student123', 'princess@student.com', 'Student');

-- --------------------------------------------------------
-- Table Structure for 'Students' (Dedicated Table Linked to Users)
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS Students (
    StudentId VARCHAR(20) PRIMARY KEY,
    UserId INT UNIQUE NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Section VARCHAR(10) NOT NULL,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- Seed Data: ACT-1B Students
INSERT IGNORE INTO Students (StudentId, UserId, FirstName, LastName, Section) VALUES 
('2026-0001', 3, 'Hailey', 'Buenaventura', 'ACT-1B'),
('2026-0002', 4, 'Jeneyah', 'Ariraya', 'ACT-1B'),
('2026-0003', 5, 'Angel', 'Laxama', 'ACT-1B'),
('2026-0004', 6, 'Angela', 'Suba', 'ACT-1B'),
('2026-0005', 7, 'Princess', 'Azada', 'ACT-1B');

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
-- Table Structure for 'Borrows'
-- Updated to reference the new Students table
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS Borrows (
    BorrowId INT AUTO_INCREMENT PRIMARY KEY,
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
