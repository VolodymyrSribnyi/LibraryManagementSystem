using Abstractions.Repositories;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly LibraryContext _libraryContext;

        public NotificationRepository(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }
        public async Task<Notification> CreateAsync(Notification notification)
        {
            using var transaction = await _libraryContext.Database.BeginTransactionAsync();
            try
            {
                var createdNotification = _libraryContext.Notifications.Add(notification).Entity;

                var user = await _libraryContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == notification.UserId);

                if (user == null)
                    throw new UserNotFoundException($"User with id {notification.UserId} not found");

                user.Notifications.Add(notification);
                await _libraryContext.SaveChangesAsync();
                transaction.Commit();
                return createdNotification;
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<Notification> GetByIdAsync(Guid notificationId)
        {
            var notification = await _libraryContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);

            return notification;
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            var notifications = _libraryContext.Notifications.Where(n => n.UserId == userId).AsEnumerable();

            return notifications;
        }
        public async Task<bool> DeleteAsync(Guid notificationId)
        {
            var notification = _libraryContext.Notifications.FirstOrDefault(n => n.Id == notificationId);

            _libraryContext.Notifications.Remove(notification);

            return true;
        }

        public async Task<bool> MarkNotificationAsReadAsync(Guid notificationId)
        {
            var notification = _libraryContext.Notifications.FirstOrDefault(n => n.Id == notificationId);

            notification.IsRead = true;

            await _libraryContext.SaveChangesAsync();

            return true;
        }
    }
}
