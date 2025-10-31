using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions.Repositories
{
    /// <summary>
    /// Defines methods for managing book notification subscription requests.
    /// </summary>
    /// <remarks>This interface provides functionality to handle operations related to book notification
    /// subscriptions, such as retrieving unnotified subscribers, creating and removing subscriptions, and checking
    /// subscription status.</remarks>
    public interface IBookNotificationRequestRepository
    {
        /// <summary>
        /// Retrieves a collection of subscribers who have not yet been notified about the specified book.
        /// </summary>
        /// <param name="bookId">The unique identifier of the book for which to find unnotified subscribers.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of 
        /// <see cref="BookNotificationRequest"/> objects representing the unnotified subscribers.</returns>
        Task<IEnumerable<BookNotificationRequest>> GetUnnotifiedSubscribersAsync(Guid bookId);

        /// <summary>
        /// Marks the specified request as notified.
        /// </summary>
        /// <param name="requestId">The unique identifier of the request to mark as notified.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the request was successfully marked as notified; otherwise, <see langword="false"/>.</returns>
        Task<bool> MarkAsNotifiedAsync(Guid requestId);

        /// <summary>
        /// Creates a new book notification subscription for a user.
        /// </summary>
        /// <param name="bookNotificationRequest">The book notification request containing subscription details.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="BookNotificationRequest"/> object.</returns>
        Task<BookNotificationRequest> CreateSubscriptionAsync(BookNotificationRequest bookNotificationRequest);

        /// <summary>
        /// Checks whether a specific user is subscribed to notifications for a specific book.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="bookId">The unique identifier of the book.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the user is subscribed; otherwise, <see langword="false"/>.</returns>
        Task<bool> IsUserSubscribedAsync(Guid userId, Guid bookId);

        /// <summary>
        /// Retrieves all book notification subscriptions in the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of all <see cref="BookNotificationRequest"/> objects.</returns>
        Task<IEnumerable<BookNotificationRequest>> GetAllSubscriptionsAsync();

        /// <summary>
        /// Retrieves all book notification subscriptions for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose subscriptions to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="BookNotificationRequest"/> objects for the specified user.</returns>
        Task<IEnumerable<BookNotificationRequest>> GetUserSubscriptionsAsync(Guid userId);

        /// <summary>
        /// Removes a book notification subscription.
        /// </summary>
        /// <param name="requestId">The unique identifier of the subscription request to remove.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the subscription was successfully removed; otherwise, <see langword="false"/>.</returns>
        Task<bool> RemoveSubscriptionAsync(Guid requestId);
    }
}
