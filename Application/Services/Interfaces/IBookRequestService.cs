using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBookRequestService
    {
        Task<BookNotificationRequest> CreateBookNotificationAsync(Guid userId, Guid bookId);
        Task<IEnumerable<BookNotificationRequest>> GetUnnotifiedSubscribersAsync(Guid bookId);
        Task<bool> IsUserSubscribedAsync(Guid userId, Guid bookId);
        Task<bool> MarkAsNotifiedAsync(Guid requestId);
        Task<IEnumerable<BookNotificationRequest>> GetUserSubscriptionsAsync(Guid userId);
        Task<bool> RemoveSubscriptionAsync(Guid requestId);
    }
}
