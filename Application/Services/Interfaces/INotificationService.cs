using Application.DTOs.Notitfications;
using Application.ErrorHandling;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    /// <summary>
    /// Defines a contract for managing notification operations, including creating and sending various types of
    /// notifications to users.
    /// </summary>
    /// <remarks>This service provides methods for creating notifications and sending specific notification types
    /// such as book availability alerts, reservation confirmations, reminders, and system updates. Implementations
    /// of this interface are expected to handle the business logic for notification creation, delivery, and
    /// management.</remarks>
    public interface INotificationService
    {
        /// <summary>
        /// Creates a new notification in the system.
        /// </summary>
        /// <param name="createNotificationDTO">The data transfer object containing the information for the new notification.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetNotificationDTO"/> representing the created notification.</returns>
        Task<Result<GetNotificationDTO>> CreateNotification(CreateNotificationDTO createNotificationDTO);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        Task<Result> DeleteNotificationAsync(Guid notificationId);

        /// <summary>
        /// Sends a notification to a user indicating that a requested book is now available.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to notify.</param>
        /// <param name="bookId">The unique identifier of the book that is now available.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetNotificationDTO"/> representing the sent notification.</returns>
        Task<Result<GetNotificationDTO>> SendBookAvailableNotificationAsync(Guid userId, Guid bookId);

        /// <summary>
        /// Sends a reservation confirmation notification to a user.
        /// </summary>
        /// <param name="reservationId">The unique identifier of the reservation.</param>
        /// <param name="userId">The unique identifier of the user to notify.</param>
        /// <param name="bookId">The unique identifier of the reserved book.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetNotificationDTO"/> representing the sent notification.</returns>
        Task<Result<GetNotificationDTO>> SendReservationConfirmationAsync(Guid reservationId, Guid userId, Guid bookId);

        /// <summary>
        /// Marks a specific notification as read.
        /// </summary>
        /// <param name="notificationId">The unique identifier of the notification to mark as read.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the notification was successfully marked as read; otherwise, <see langword="false"/>.</returns>
        Task<Result> MarkNotificationAsReadAsync(Guid notificationId);

        /// <summary>
        /// Retrieves all notifications for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose notifications to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="GetNotificationDTO"/> objects for the specified user.</returns>
        Task<IEnumerable<GetNotificationDTO>> GetUserNotificationsAsync(Guid userId);

        /// <summary>
        /// Sends a reminder notification to a user about an upcoming book due date.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to notify.</param>
        /// <param name="bookId">The unique identifier of the book that is due.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetNotificationDTO"/> representing the sent notification.</returns>
        Task<Result<GetNotificationDTO>> SendBookDueReminder(Guid userId, Guid bookId);

        /// <summary>
        /// Sends a notification to a user about a newly arrived book in the library.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to notify.</param>
        /// <param name="bookId">The unique identifier of the newly arrived book.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetNotificationDTO"/> representing the sent notification.</returns>
        Task<Result<GetNotificationDTO>> SendNewBookArrival(Guid userId, Guid bookId);

        /// <summary>
        /// Sends a notification to a user about their library card expiration.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to notify.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetNotificationDTO"/> representing the sent notification.</returns>
        Task<Result<GetNotificationDTO>> SendLibraryCardExpiry(Guid userId);

        /// <summary>
        /// Sends a general update notification to a user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to notify.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetNotificationDTO"/> representing the sent notification.</returns>
        Task<Result<GetNotificationDTO>> SendGeneralUpdate(Guid userId);

        /// <summary>
        /// Sends a system alert notification to a user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to notify.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetNotificationDTO"/> representing the sent notification.</returns>
        Task<Result<GetNotificationDTO>> SendSystemAlert(Guid userId);
    }
}
