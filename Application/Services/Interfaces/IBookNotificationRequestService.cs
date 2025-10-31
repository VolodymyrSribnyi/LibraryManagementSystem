using Application.DTOs.BookNotificationRequests;
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
    /// Defines a contract for managing book notification subscription services.
    /// </summary>
    /// <remarks>This service provides methods for handling book notification subscriptions, including creating
    /// subscriptions, retrieving unnotified subscribers, and managing subscription status. Implementations of this
    /// interface are expected to handle the business logic and coordination with the underlying data access layer.</remarks>
    public interface IBookNotificationRequestService
    {
        /// <summary>
        /// Creates a new book notification subscription for a user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user subscribing to notifications.</param>
        /// <param name="bookId">The unique identifier of the book to subscribe to.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="BookNotificationRequestDTO"/> object.</returns>
        Task<Result<BookNotificationRequestDTO>> CreateBookNotificationAsync(Guid userId, Guid bookId);

        /// <summary>
        /// Retrieves a collection of subscribers who have not yet been notified about the specified book.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book for which to find unnotified subscribers.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="BookNotificationRequestDTO"/> objects representing the unnotified subscribers.</returns>
        Task<Result<IEnumerable<BookNotificationRequestDTO>>> GetUnnotifiedSubscribersAsync(Guid bookId);

        /// <summary>
        /// Checks whether a specific user is subscribed to notifications for a specific book.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the user is subscribed; otherwise, <see langword="false"/>.</returns>
        Task<Result<bool>> IsUserSubscribedAsync(Guid userId, Guid bookId);

        /// <summary>
        /// Marks the specified notification request as notified.
        /// </summary>
        /// <param name="requestId">The unique identifier of the request to mark as notified.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the request was successfully marked as notified; otherwise, <see langword="false"/>.</returns>
        Task<Result<bool>> MarkAsNotifiedAsync(Guid requestId);

        /// <summary>
        /// Retrieves all book notification subscriptions for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose subscriptions to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Application.DTOs.BookNotificationRequests.BookNotificationRequestDTO"/> objects for the specified user.</returns>
        Task<Result<IEnumerable<BookNotificationRequestDTO>>> GetUserSubscriptionsAsync(Guid userId);

        /// <summary>
        /// Removes a book notification subscription.
        /// </summary>
        /// <param name="requestId">The unique identifier of the subscription request to remove.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the subscription was successfully removed; otherwise, <see langword="false"/>.</returns>
        Task<Result<bool>> RemoveSubscriptionAsync(Guid requestId);
    }
}
