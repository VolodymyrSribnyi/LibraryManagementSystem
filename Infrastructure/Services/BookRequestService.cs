using Application.Services.Interfaces;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    /// <summary>
    /// Provides functionality for managing book notification requests, including creating subscriptions,  retrieving
    /// subscribers, and managing notification statuses.
    /// </summary>
    /// <remarks>This service acts as a mediator between the book-related operations and the repository layer,
    /// ensuring that business rules are enforced when handling book notification requests. It includes  methods for
    /// creating subscriptions, checking subscription status, retrieving unnotified subscribers,  and managing
    /// notification states.</remarks>
    public class BookRequestService : IBookRequestService
    {
        private readonly IBookService _bookService;
        private readonly IBookRequestRepository _bookRequestRepository;
        private readonly ILogger<BookRequestService> _logger;
        public BookRequestService(IBookService bookService, IBookRequestRepository bookRequestRepository, ILogger<BookRequestService> logger)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _bookRequestRepository = bookRequestRepository ?? throw new ArgumentNullException(nameof(bookRequestRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<BookNotificationRequest> CreateBookNotificationAsync(Guid userId, Guid bookId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if (bookId == Guid.Empty)
            {
                throw new ArgumentException("Book ID cannot be empty.", nameof(bookId));
            }

            var book = await _bookService.GetByIdAsync(bookId);

            if (book == null)
            {
                throw new BookNotFoundException($"Book with ID {bookId} not found.");
            }

            var subscription = new BookNotificationRequest
            {
                UserId = userId,
                BookId = bookId,
                IsNotified = false,
            };

            if (await IsUserSubscribedAsync(userId, bookId))
            {
                throw new UserIsSubscribedException("User is already subscribed to notifications for this book.");
            }

            await _bookRequestRepository.CreateSubscriptionAsync(subscription);

            return subscription;
        }
        public async Task<IEnumerable<BookNotificationRequest>> GetUnnotifiedSubscribersAsync(Guid bookId)
        {
            if (bookId == Guid.Empty)
            {
                throw new ArgumentNullException("Book ID cannot be empty.", nameof(bookId));
            }
            var book = await _bookService.GetByIdAsync(bookId);

            if (book == null)
            {
                throw new BookNotFoundException($"Book with ID {bookId} not found.");
            }

            return await _bookRequestRepository.GetUnnotifiedSubscribersAsync(bookId);
        }

        public async Task<bool> IsUserSubscribedAsync(Guid userId, Guid bookId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException("User ID cannot be empty.", nameof(userId));
            }
            if (bookId == Guid.Empty)
            {
                throw new ArgumentNullException("Book ID cannot be empty.", nameof(bookId));
            }

            var book = await _bookService.GetByIdAsync(bookId);

            if (book == null)
            {
                throw new BookNotFoundException($"Book with ID {bookId} not found.");
            }

            return await _bookRequestRepository.IsUserSubscribedAsync(userId, bookId);
        }

        public Task<bool> MarkAsNotifiedAsync(Guid requestId)
        {
            if (requestId == Guid.Empty)
            {
                throw new ArgumentNullException("Request ID cannot be empty.", nameof(requestId));
            }

            return _bookRequestRepository.MarkAsNotifiedAsync(requestId);
        }

        public async Task<IEnumerable<BookNotificationRequest>> GetAllSubscriptionsAsync()
        {
            var subscriptions = await _bookRequestRepository.GetAllSubscriptionsAsync();

            if(subscriptions == null || !subscriptions.Any())
            {
                _logger.LogInformation("No subscriptions found in the system.");
                return Enumerable.Empty<BookNotificationRequest>();
            }

            return subscriptions;
        }
        public Task<IEnumerable<BookNotificationRequest>> GetUserSubscriptionsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("GetUserSubscriptionsAsync called with empty userId.");
                throw new ArgumentNullException("User ID cannot be empty.", nameof(userId));
            }
            return _bookRequestRepository.GetUserSubscriptionsAsync(userId);
        }

        public async Task<bool> RemoveSubscriptionAsync(Guid requestId)
        {
            if(requestId == Guid.Empty) 
                throw new ArgumentNullException("Request Id cannot be empty",nameof(requestId));

            var result = await _bookRequestRepository.RemoveSubscriptionAsync(requestId);

            return result;
        }
    }
}
