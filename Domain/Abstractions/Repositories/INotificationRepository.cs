using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Repositories
{
    /// <summary>
    /// Defines a contract for managing and retrieving notifications in the system.
    /// </summary>
    /// <remarks>This interface provides methods for creating, retrieving, and updating notifications.
    /// Implementations of this interface are responsible for handling the persistence and retrieval of notification
    /// data, as well as marking notifications as read.</remarks>
    public interface INotificationRepository
    {
        public Task<Notification> CreateAsync(Notification notification);
        
        public Task<bool> MarkNotificationAsReadAsync(Guid notificationId);
        public Task<Notification> GetByIdAsync(Guid notificationId);
        public Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId);
    }
}
