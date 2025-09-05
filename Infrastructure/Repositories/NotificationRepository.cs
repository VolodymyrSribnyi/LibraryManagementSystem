using Abstractions.Repositories;
using Domain.Entities;
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
            var createdNotification = _libraryContext.Notifications.Add(notification).Entity;

            await _libraryContext.SaveChangesAsync();

            return createdNotification;
        }

        public async Task<Notification> GetByIdAsync(Guid notificationId)
        {
            var notification = await _libraryContext.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);

            return notification;
        }

        public Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            var notifications = _libraryContext.Notifications.Where(n => n.UserId == userId).AsEnumerable();

            return Task.FromResult(notifications);
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
