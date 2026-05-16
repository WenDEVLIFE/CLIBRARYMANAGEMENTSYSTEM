using System;
using System.Threading.Tasks;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Models;
using System.Collections.Generic;

namespace LibraryManagementSystem.Utils
{
    public class NotificationService
    {
        private readonly LoanRepository _loanRepository;
        private readonly NotificationRepository _notificationRepository;

        public NotificationService()
        {
            _loanRepository = new LoanRepository();
            _notificationRepository = new NotificationRepository();
        }

        public async Task ProcessDueNotificationsAsync()
        {
            // 1. Check for Overdue Loans
            var overdueLoans = await _loanRepository.GetOverdueLoansAsync();
            foreach (var loan in overdueLoans)
            {
                string message = $"OVERDUE: The book '{loan.BookTitle}' was due on {loan.DueDate:MMM dd, yyyy}. Please return it immediately.";
                await CreateNotificationIfNotExists(loan.StudentId, message, NotificationType.ReturnReminder);
            }

            // 2. Check for Loans due within 3 days
            var upcomingLoans = await _loanRepository.GetUpcomingDueLoansAsync(3);
            foreach (var loan in upcomingLoans)
            {
                string message = $"REMINDER: The book '{loan.BookTitle}' is due soon ({loan.DueDate:MMM dd, yyyy}).";
                await CreateNotificationIfNotExists(loan.StudentId, message, NotificationType.ReturnReminder);
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
