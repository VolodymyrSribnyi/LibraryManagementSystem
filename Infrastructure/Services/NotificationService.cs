using Abstractions.Repositories;
using Application.DTOs.Notitfications;
using Application.ErrorHandling;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    /// <summary>
    /// Provides functionality for managing notifications, including creating various types of notifications
    /// and retrieving user notifications.
    /// </summary>
    /// <remarks>
    /// This service handles all notification-related operations including book availability alerts,
    /// reservation confirmations, due date reminders, and system notifications.
    /// </remarks>
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IReservingBookRepository _reservingBookRepository;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationRepository notificationRepository, IReservingBookRepository reservingBookRepository,
            IMapper mapper, ILogger<NotificationService> logger, IBookService bookService)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _reservingBookRepository = reservingBookRepository ?? throw new ArgumentNullException(nameof(reservingBookRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        }

        public async Task<Result<GetNotificationDTO>> CreateNotification(CreateNotificationDTO createNotificationDTO)
        {
            if (createNotificationDTO == null)
            {
                _logger.LogWarning("CreateNotification called with null CreateNotificationDTO.");
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            var notificationToCreate = _mapper.Map<Notification>(createNotificationDTO);
            var notification = await _notificationRepository.CreateAsync(notificationToCreate);

            if (notification == null)
            {
                _logger.LogError($"Failed to create notification of type {createNotificationDTO.NotificationType}");
                return Result<GetNotificationDTO>.Failure(Errors.NotificationCreationFailed);
            }

            _logger.LogInformation($"Successfully created notification with ID: {notification.Id}");
            return Result<GetNotificationDTO>.Success(_mapper.Map<GetNotificationDTO>(notification));
        }
        public async Task<Result> DeleteNotificationAsync(Guid notificationId)
        {
            if (IsInvalidGuid(notificationId, nameof(notificationId)))
                return Result.Failure(Errors.NullData);

            var success = await _notificationRepository.DeleteAsync(notificationId);
            return success
                ? Result.Success()
                : Result.Failure(Errors.NotificationDeletionFailed);
        }
        public async Task<IEnumerable<GetNotificationDTO>> GetUserNotificationsAsync(Guid userId)
        {
            if (IsInvalidGuid(userId, nameof(userId)))
            {
                return Enumerable.Empty<GetNotificationDTO>();
            }

            var notifications = await _notificationRepository.GetUserNotificationsAsync(userId);

            if (notifications == null || !notifications.Any())
            {
                _logger.LogInformation($"No notifications found for user with ID: {userId}");
                return Enumerable.Empty<GetNotificationDTO>();
            }

            _logger.LogInformation($"Retrieved {notifications.Count()} notifications for user ID: {userId}");
            return _mapper.Map<IEnumerable<GetNotificationDTO>>(notifications);
        }

        public async Task<Result> MarkNotificationAsReadAsync(Guid notificationId)
        {
            if (IsInvalidGuid(notificationId, nameof(notificationId)))
            {
                return Result.Failure(Errors.NullData);
            }

            var notification = await _notificationRepository.GetByIdAsync(notificationId);

            if (notification == null)
            {
                _logger.LogInformation($"Notification with ID {notificationId} not found.");
                return Result.Failure(Errors.NotificationNotFound);
            }

            var success = await _notificationRepository.MarkNotificationAsReadAsync(notification.Id);

            if (!success)
            {
                _logger.LogError($"Failed to mark notification with ID {notificationId} as read");
                return Result.Failure(Errors.FailedToMarkAsRead);
            }

            _logger.LogInformation($"Successfully marked notification with ID {notificationId} as read");
            return Result.Success();
        }

        public async Task<Result<GetNotificationDTO>> SendBookAvailableNotificationAsync(Guid userId, Guid bookId)
        {
            if (IsInvalidGuid(userId, nameof(userId)) || IsInvalidGuid(bookId, nameof(bookId)))
            {
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            var book = await _bookService.GetByIdAsync(bookId);

            if (book.Value == null)
            {
                _logger.LogInformation($"Book with ID {bookId} not found.");
                return Result<GetNotificationDTO>.Failure(Errors.BookNotFound);
            }

            var notification = await SendAsync(userId, bookId, $"The book '{book.Value.Title}' you requested is now available.", NotificationType.BookAvailable);

            _logger.LogInformation($"Successfully sent book available notification for book '{book.Value.Title}' to user {userId}");
            return Result<GetNotificationDTO>.Success(_mapper.Map<GetNotificationDTO>(notification.Value));
        }

        public async Task<Result<GetNotificationDTO>> SendReservationConfirmationAsync(Guid reservationId, Guid userId, Guid bookId)
        {
            if (IsInvalidGuid(reservationId, nameof(reservationId)) || IsInvalidGuid(userId, nameof(userId))
                || IsInvalidGuid(userId, nameof(userId)))
            {
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            var reservation = await _reservingBookRepository.GetByIdAsync(reservationId);

            if (reservation == null)
            {
                _logger.LogInformation($"Reservation with ID {reservationId} not found.");
                return Result<GetNotificationDTO>.Failure(Errors.ReservationNotFound);
            }

            var reservationBook = await _bookService.GetByIdAsync(reservation.BookId);

            if (reservationBook.Value == null)
            {
                _logger.LogInformation($"Book with id {reservation.BookId} is not found");
                return Result<GetNotificationDTO>.Failure(Errors.BookNotFound);
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = bookId,
                Message = $"Your reservation of '{reservationBook.Value.Title}' has been confirmed.",
                NotificationType = NotificationType.ReservationConfirmation,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            var createdNotification = await _notificationRepository.CreateAsync(notification);

            if (createdNotification == null)
            {
                _logger.LogError($"Failed to create reservation confirmation notification for reservation {reservationId}");
                return Result<GetNotificationDTO>.Failure(Errors.NotificationCreationFailed);
            }

            _logger.LogInformation($"Successfully sent reservation confirmation for reservation {reservationId} to user {userId}");
            return Result<GetNotificationDTO>.Success(_mapper.Map<GetNotificationDTO>(createdNotification));
        }

        public async Task<Result<GetNotificationDTO>> SendBookDueReminder(Guid userId, Guid bookId)
        {
            if (IsInvalidGuid(userId, nameof(userId)) || IsInvalidGuid(bookId, nameof(bookId)))
            {
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            var book = await _bookService.GetByIdAsync(bookId);

            if (book.Value == null)
            {
                _logger.LogInformation($"Book with ID {bookId} not found.");
                return Result<GetNotificationDTO>.Failure(Errors.BookNotFound);
            }

            var notification = await SendAsync(userId, bookId, $"This is a reminder that your borrowed book '{book.Value.Title}' is due soon.", NotificationType.BookDueReminder);

            _logger.LogInformation($"Successfully sent book due reminder for book {bookId} to user {userId}");
            return Result<GetNotificationDTO>.Success(_mapper.Map<GetNotificationDTO>(notification.Value));
        }

        public async Task<Result<GetNotificationDTO>> SendNewBookArrival(Guid userId, Guid bookId)
        {
            if (IsInvalidGuid(userId, nameof(userId)) || IsInvalidGuid(bookId, nameof(bookId)))
            {
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            var book = await _bookService.GetByIdAsync(bookId);

            if (book.Value == null)
            {
                _logger.LogInformation($"Book with ID {bookId} not found.");
                return Result<GetNotificationDTO>.Failure(Errors.BookNotFound);
            }

            var notification = await SendAsync(userId, bookId, $"A new book '{book.Value.Title}' has arrived that you might be interested in.", NotificationType.NewBookArrival);

            _logger.LogInformation($"Successfully sent new book arrival notification for book {bookId} to user {userId}");
            return Result<GetNotificationDTO>.Success(_mapper.Map<GetNotificationDTO>(notification.Value));
        }

        public async Task<Result<GetNotificationDTO>> SendLibraryCardExpiry(Guid userId)
        {
            if (IsInvalidGuid(userId, nameof(userId)))
            {
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Message = "Your library card is about to expire. Please renew it.",
                NotificationType = NotificationType.LibraryCardExpiry,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            var createdNotification = await _notificationRepository.CreateAsync(notification);

            if (createdNotification == null)
            {
                _logger.LogError($"Failed to create library card expiry notification for user {userId}");
                return Result<GetNotificationDTO>.Failure(Errors.NotificationCreationFailed);
            }

            _logger.LogInformation($"Successfully sent library card expiry notification to user {userId}");
            return Result<GetNotificationDTO>.Success(_mapper.Map<GetNotificationDTO>(createdNotification));
        }

        public async Task<Result<GetNotificationDTO>> SendGeneralUpdate(Guid userId)
        {
            if (IsInvalidGuid(userId, nameof(userId)))
            {
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            var notification = await SendAsync(userId, null, "We have some updates for you. Please check your account.", NotificationType.GeneralUpdate);

            _logger.LogInformation($"Successfully sent general update notification to user {userId}");
            return Result<GetNotificationDTO>.Success(_mapper.Map<GetNotificationDTO>(notification.Value));
        }

        public async Task<Result<GetNotificationDTO>> SendSystemAlert(Guid userId)
        {
            if (IsInvalidGuid(userId, nameof(userId)))
            {
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }
            var notification = await SendAsync(userId, null, "There is a system alert that you should be aware of.", NotificationType.SystemAlert);

            _logger.LogInformation($"Successfully sent system alert notification to user {userId}");
            return Result<GetNotificationDTO>.Success(_mapper.Map<GetNotificationDTO>(notification.Value));
        }
        private bool IsInvalidGuid(Guid id, string paramName)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning($"{paramName} is empty.");
                return true;
            }
            return false;
        }
        private Notification CreateBaseNotification(Guid userId, Guid? bookId, string message, NotificationType type)
        {
            return new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = bookId,
                Message = message,
                NotificationType = type,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
        }
        private async Task<Result<GetNotificationDTO>> SendAsync(Guid userId, Guid? bookId, string message, NotificationType type)
        {
            var notification = CreateBaseNotification(userId, bookId, message, type);
            var created = await _notificationRepository.CreateAsync(notification);

            if (created == null)
            {
                _logger.LogError($"Failed to create notification for user {userId}, type {type}");
                return Result<GetNotificationDTO>.Failure(Errors.NotificationCreationFailed);
            }

            _logger.LogInformation($"Successfully created notification '{type}' for user {userId}");
            return Result<GetNotificationDTO>.Success(_mapper.Map<GetNotificationDTO>(created));
        }
    }
}