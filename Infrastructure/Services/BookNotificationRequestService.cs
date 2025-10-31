using Application.DTOs.BookNotificationRequests;
using Application.ErrorHandling;
using Application.Services.Interfaces;
using AutoMapper;
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
    public class BookNotificationRequestService : IBookNotificationRequestService
    {
        private readonly IBookService _bookService;
        private readonly IBookNotificationRequestRepository _bookRequestRepository;
        private readonly ILogger<BookNotificationRequestService> _logger;
        private readonly IMapper _mapper;
        public BookNotificationRequestService(IBookService bookService, IBookNotificationRequestRepository bookRequestRepository,
            ILogger<BookNotificationRequestService> logger, IMapper mapper)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _bookRequestRepository = bookRequestRepository ?? throw new ArgumentNullException(nameof(bookRequestRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(_mapper));
        }
        public async Task<Result<BookNotificationRequestDTO>> CreateBookNotificationAsync(Guid userId, Guid bookId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("CreateBookNotificationAsync called with empty userId.");
                return Result<BookNotificationRequestDTO>.Failure(Errors.NullData);
            }
            if (bookId == Guid.Empty)
            {
                _logger.LogWarning("CreateBookNotificationAsync called with empty bookId.");
                return Result<BookNotificationRequestDTO>.Failure(Errors.NullData);
            }

            var book = await _bookService.GetByIdAsync(bookId);

            if (book == null)
            {
                _logger.LogInformation($"Book with ID {bookId} not found.");
                return Result<BookNotificationRequestDTO>.Failure(Errors.BookNotFound);
            }

            var subscription = new BookNotificationRequest
            {
                UserId = userId,
                BookId = bookId
                //IsNotified = false,
            };

            var isUserSubscribed = await IsUserSubscribedAsync(userId, bookId);


            if (isUserSubscribed.IsFailure)
            {
                return Result<BookNotificationRequestDTO>.Failure(isUserSubscribed.Error);
            }

            if (isUserSubscribed.Value)
            {
                _logger.LogInformation($"User with ID {userId} is already subscribed to notifications for book ID {bookId}.");
                return Result<BookNotificationRequestDTO>.Failure(Errors.UserSubscribed);
            }


            await _bookRequestRepository.CreateSubscriptionAsync(subscription);

            _logger.LogInformation($"User with ID {userId} subscribed to notifications for book ID {bookId}.");
            return Result<BookNotificationRequestDTO>.Success(_mapper.Map<BookNotificationRequestDTO>(subscription));
        }
        public async Task<Result<IEnumerable<BookNotificationRequestDTO>>> GetUnnotifiedSubscribersAsync(Guid bookId)
        {
            if (bookId == Guid.Empty)
            {
                _logger.LogWarning("GetUnnotifiedSubscribersAsync called with empty bookId.");
                return Result<IEnumerable<BookNotificationRequestDTO>>.Failure(Errors.NullData);
            }
            var book = await _bookService.GetByIdAsync(bookId);

            if (book == null)
            {
                _logger.LogInformation($"Book with ID {bookId} not found.");
                return Result<IEnumerable<BookNotificationRequestDTO>>.Failure(Errors.BookNotFound);
            }

            var unnotifiedSubscribers = await _bookRequestRepository.GetUnnotifiedSubscribersAsync(bookId);

            _logger.LogInformation($"Retrieved {unnotifiedSubscribers.Count()} unnotified subscribers for book ID {bookId}.");
            return Result<IEnumerable<BookNotificationRequestDTO>>.Success(_mapper.Map<IEnumerable<BookNotificationRequestDTO>>(unnotifiedSubscribers));
        }

        public async Task<Result<bool>> IsUserSubscribedAsync(Guid userId, Guid bookId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("IsUserSubscribedAsync called with empty userId.");
                return Result<bool>.Failure(Errors.NullData);
            }
            if (bookId == Guid.Empty)
            {
                _logger.LogWarning("IsUserSubscribedAsync called with empty bookId.");
                return Result<bool>.Failure(Errors.NullData);
            }

            var book = await _bookService.GetByIdAsync(bookId);

            var isUserSubscribed = await _bookRequestRepository.IsUserSubscribedAsync(userId, bookId);

            _logger.LogInformation($"User with ID {userId} subscription status for book ID {bookId}: {isUserSubscribed}.");
            return Result<bool>.Success(isUserSubscribed);
        }

        public async Task<Result<bool>> MarkAsNotifiedAsync(Guid requestId)
        {
            if (requestId == Guid.Empty)
            {
                _logger.LogWarning("Request Id cannot be empty");
                return Result<bool>.Failure(Errors.NullData);
            }

            var result = await _bookRequestRepository.MarkAsNotifiedAsync(requestId);

            _logger.LogInformation($"Marked request ID {requestId} as notified: {result}.");
            return Result<bool>.Success(result);
        }

        public async Task<Result<IEnumerable<BookNotificationRequestDTO>>> GetAllSubscriptionsAsync()
        {
            var subscriptions = await _bookRequestRepository.GetAllSubscriptionsAsync();

            if (subscriptions == null || !subscriptions.Any())
            {
                _logger.LogInformation($"{Errors.SubscriptionsNotFound.Code} {Errors.SubscriptionsNotFound.Description}");
                return Result<IEnumerable<BookNotificationRequestDTO>>.Success(Enumerable.Empty<BookNotificationRequestDTO>());
            }

            _logger.LogInformation($"Retrieved {subscriptions.Count()} total subscriptions.");
            return Result<IEnumerable<BookNotificationRequestDTO>>.Success(_mapper.Map<IEnumerable<BookNotificationRequestDTO>>(subscriptions));
        }
        public async Task<Result<IEnumerable<BookNotificationRequestDTO>>> GetUserSubscriptionsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("GetUserSubscriptionsAsync called with empty userId.");
                return Result<IEnumerable<BookNotificationRequestDTO>>.Failure(Errors.NullData);
            }

            var userSubscriptions = await _bookRequestRepository.GetUserSubscriptionsAsync(userId);

            _logger.LogInformation($"Retrieved {userSubscriptions.Count()} subscriptions for user ID {userId}.");
            return Result<IEnumerable<BookNotificationRequestDTO>>.Success(_mapper.Map<IEnumerable<BookNotificationRequestDTO>>(userSubscriptions));
        }

        public async Task<Result<bool>> RemoveSubscriptionAsync(Guid requestId)
        {
            if (requestId == Guid.Empty)
            {
                _logger.LogWarning("Request Id cannot be empty");
                return Result<bool>.Failure(Errors.NullData);
            }

            var result = await _bookRequestRepository.RemoveSubscriptionAsync(requestId);

            _logger.LogInformation($"Removed subscription with request ID {requestId}: {result}.");
            return Result<bool>.Success(result);
        }
    }
}
