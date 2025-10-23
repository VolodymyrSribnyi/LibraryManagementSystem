using Abstractions.Repositories;
using Application.Services.Interfaces;
using Domain.Abstractions.Repositories;
using Domain.Entities;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.EventHadlers
{
    /// <summary>
    /// Handles the <see cref="BookBecameAvailableEvent"/> by notifying subscribers who have requested the book.
    /// </summary>
    /// <remarks>This handler retrieves subscribers who have requested notifications for a specific book and
    /// sends them notifications when the book becomes available. After sending notifications, it marks the subscribers
    /// as notified and removes their subscriptions.</remarks>
    public class BookAvailabilityNotificationHandler : IDomainEventHandler<BookBecameAvailableEvent>
    {
        private readonly IBookRequestRepository _bookRequestRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<BookAvailabilityNotificationHandler> _logger;
        public BookAvailabilityNotificationHandler(IBookRequestRepository bookRequestRepository, 
            INotificationRepository notificationRepository, ILogger<BookAvailabilityNotificationHandler> logger,
            INotificationService notificationService)
        {
            _bookRequestRepository = bookRequestRepository;
            _logger = logger;
            _notificationService = notificationService;
        }
        public async Task HandleAsync(BookBecameAvailableEvent domainEvent)
        {
            // Fetch subscribers who haven't been notified yet
            IEnumerable<BookNotificationRequest> subscribers = await _bookRequestRepository
                .GetUnnotifiedSubscribersAsync(domainEvent.BookId);

            // Send notifications to subscribers and mark as notified
            foreach (var subscriber in subscribers)
            {
                await _notificationService.SendBookAvailableNotificationAsync(subscriber.UserId, domainEvent.BookId);
                await _bookRequestRepository.MarkAsNotifiedAsync(subscriber.Id);
                await _bookRequestRepository.RemoveSubscriptionAsync(subscriber.Id);
            }

            _logger.LogInformation($"Sent notifications for book {domainEvent.BookId} to {subscribers.Count()} users");

        }
    }
}
