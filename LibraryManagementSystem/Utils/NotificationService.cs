using System;
using System.Threading.Tasks;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Models;
using System.Collections.Generic;

namespace LibraryManagementSystem.Utils
{
    public class NotificationService
    {
        private readonly BorrowRepository _BorrowRepository;
        private readonly NotificationRepository _notificationRepository;

        public NotificationService()
        {
            _BorrowRepository = new BorrowRepository();
            _notificationRepository = new NotificationRepository();
        }

        public async Task ProcessDueNotificationsAsync()
        {
            // 1. Check for Overdue Borrows
            var overdueBorrows = await _BorrowRepository.GetOverdueBorrowsAsync();
            foreach (var Borrow in overdueBorrows)
            {
                string message = $"OVERDUE: The book '{Borrow.BookTitle}' was due on {Borrow.DueDate:MMM dd, yyyy}. Please return it immediately.";
                await CreateNotificationIfNotExists(Borrow.StudentId, message, NotificationType.ReturnReminder);
            }

            // 2. Check for Borrows due within 3 days
            var upcomingBorrows = await _BorrowRepository.GetUpcomingDueBorrowsAsync(3);
            foreach (var Borrow in upcomingBorrows)
            {
                string message = $"REMINDER: The book '{Borrow.BookTitle}' is due soon ({Borrow.DueDate:MMM dd, yyyy}).";
                await CreateNotificationIfNotExists(Borrow.StudentId, message, NotificationType.ReturnReminder);
            }
        }

        private async Task CreateNotificationIfNotExists(string studentId, string message, NotificationType type)
        {
            var existing = await _notificationRepository.GetByStudentIdAsync(studentId);
            // Check if a similar notification was sent in the last 24 hours
            if (!existing.Exists(n => n.Message == message && n.CreatedAt > DateTime.Now.AddDays(-1)))
            {
                await _notificationRepository.AddNotificationAsync(new Notification
                {
                    StudentId = studentId,
                    Message = message,
                    Type = type
                });
            }
        }
    }
}

