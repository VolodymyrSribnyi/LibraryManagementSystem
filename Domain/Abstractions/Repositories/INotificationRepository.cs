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
        /// <summary>
        /// Creates a new notification in the system.
        /// </summary>
        /// <param name="notification">The notification object to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="Notification"/> object.</returns>
        Task<Notification> CreateAsync(Notification notification);

        /// <summary>
        /// Marks a specific notification as read.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification to mark as read.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the notification was successfully marked as read; otherwise, <see langword="false"/>.</returns>
        Task<bool> MarkNotificationAsReadAsync(Guid notificationId);

        /// <summary>
        /// Retrieves a notification by its unique identifier.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Notification"/> object if found; otherwise, <see langword="null"/>.</returns>
        Task<Notification> GetByIdAsync(Guid notificationId);

        /// <summary>
        /// Retrieves all notifications for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose notifications to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Notification"/> objects for the specified user.</returns>
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId);
        Task<bool> DeleteAsync(Guid notificationId);
    }
}
