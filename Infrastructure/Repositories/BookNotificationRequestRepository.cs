using Domain.Abstractions.Repositories;
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
    public class BookNotificationRequestRepository : IBookNotificationRequestRepository
    {
        private readonly LibraryContext _libraryContext;
        public BookNotificationRequestRepository(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }
        //HACK transaction
        public async Task<BookNotificationRequest> CreateSubscriptionAsync(BookNotificationRequest bookNotificationRequest)
        {
            using var transaction = await _libraryContext.Database.BeginTransactionAsync();
            try
            {
                bookNotificationRequest.User = _libraryContext.Users.FirstOrDefault(u => u.Id == bookNotificationRequest.UserId);
                bookNotificationRequest.Book = _libraryContext.Books.FirstOrDefault(b => b.Id == bookNotificationRequest.BookId);
                bookNotificationRequest.User.BookSubscriptions.Add(bookNotificationRequest);

                await _libraryContext.BookNotificationRequests.AddAsync(bookNotificationRequest);

                await _libraryContext.SaveChangesAsync();
                transaction.Commit();
                return bookNotificationRequest;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException("Unable to create subscription");
            }
        }

        public async Task<IEnumerable<BookNotificationRequest>> GetAllSubscriptionsAsync()
        {
            return await _libraryContext.BookNotificationRequests.ToListAsync();
        }

        public async Task<IEnumerable<BookNotificationRequest>> GetUnnotifiedSubscribersAsync(Guid bookId)
        {
            var subscribers = await _libraryContext.BookNotificationRequests
                .Where(r => r.BookId == bookId && !r.IsNotified)
                .ToListAsync();

            return subscribers;
        }

        public async Task<IEnumerable<BookNotificationRequest>> GetUserSubscriptionsAsync(Guid userId)
        {
            var subscriptions = await _libraryContext.BookNotificationRequests
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return subscriptions;
        }

        public async Task<bool> IsUserSubscribedAsync(Guid userId, Guid bookId)
        {
            var user = await _libraryContext.BookNotificationRequests
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId && !r.IsNotified);

            if (user == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> MarkAsNotifiedAsync(Guid requestId)
        {
            var request = await _libraryContext.BookNotificationRequests
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                return false;
            }

            request.IsNotified = true;
            _libraryContext.BookNotificationRequests.Update(request);
            await _libraryContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveSubscriptionAsync(Guid requestId)
        {
            var request = await _libraryContext.BookNotificationRequests
                .FirstOrDefaultAsync(r => r.Id == requestId);

            _libraryContext.BookNotificationRequests.Remove(request);

            await _libraryContext.SaveChangesAsync();

            return true;
        }
    }
}
