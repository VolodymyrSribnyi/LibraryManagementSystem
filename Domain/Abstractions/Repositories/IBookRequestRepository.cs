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
    public interface IBookRequestRepository
    {
        Task<IEnumerable<BookNotificationRequest>> GetUnnotifiedSubscribersAsync(Guid bookId);
        Task<bool> MarkAsNotifiedAsync(Guid requestId);
        Task<BookNotificationRequest> CreateSubscriptionAsync(BookNotificationRequest bookNotificationRequest);
        Task<bool> IsUserSubscribedAsync(Guid userId,Guid bookId);
        Task<IEnumerable<BookNotificationRequest>> GetAllSubscriptionsAsync();
        Task<IEnumerable<BookNotificationRequest>> GetUserSubscriptionsAsync(Guid userId);
        Task<bool> RemoveSubscriptionAsync(Guid requestId);
    }
}
