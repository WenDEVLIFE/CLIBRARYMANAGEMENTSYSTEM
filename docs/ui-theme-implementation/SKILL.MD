# UI Theme Implementation Plan

This document describes the visual design and user interface layout for the Library Management System, inspired by the "Scholar Central" theme.

## 1. Visual Style & Color Palette

The theme will focus on a clean, professional, and academic aesthetic.

*   **Primary Header Background:** A soft teal/light-blue gradient (e.g., HSL(190, 30%, 90%) to HSL(190, 30%, 80%)).
*   **Main Content Area:** Solid white (#FFFFFF) for maximum readability.
*   **Typography:** Modern sans-serif fonts (e.g., "Segoe UI", "Roboto", or "Inter").
*   **Accent Colors:** 
    - Dark Teal (#2F4F4F) for headings and important labels.
    - Soft Grey (#F5F5F5) for section backgrounds and card borders.

## 2. Component Layout

### Header & Navigation
- **Header:** Centered title "Scholar Central" with the subtext "Library Management System".
- **Role Indicator:** A small badge in the corner showing the current logged-in user role (Admin, Librarian, or Student).

### Dashboard Overview (Inspired by Stats Bar)
A horizontal bar featuring icons with real-time counts, tailored to the user's role:
- **Admin:** Displays counts for "Total Books", "Librarians", and "Students".
- **Librarian:** Displays "Available Books" and "Active Loans".
- **Student:** Displays "Borrowed Books" and "Due Soon".

### Main Features (Cards/Modules)
- **Featured Collections/Books:** Grid layout featuring book covers or placeholder images with titles and descriptions.
- **Call-to-Action Boxes:** Styled boxes for specific role actions (e.g., "Add New Book" for Admin, "Log a Loan" for Librarian).

## 3. Role-Specific UI Implementation

### Admin Interface
- **Management Console:** A clean table view for managing books and user accounts.
- **Quick Action Sidebar:** "Create Librarian Account" and "Add New Book" forms styled with soft borders and clear labels.

### Librarian Interface
- **Inventory View:** A simplified grid of available books (view-only).
- **Loan Management System:** A dedicated form to input borrower info, borrowing date, and return date. Uses date-picker components for accuracy.

### Student Interface
- **Notification Center:** A prominent feed at the top of the dashboard showing borrow confirmations and return date reminders.
- **My History:** A simple list of books currently borrowed and their respective due dates.

## 4. Design Principles
- **Clarity:** Use plenty of white space to separate different sections.
- **Visual Cues:** Use icons (Book, User, Bell) consistently to help users navigate.
- **Responsiveness:** The layout should adapt to different screen sizes, maintaining the clean horizontal flow of the dashboard stats.
