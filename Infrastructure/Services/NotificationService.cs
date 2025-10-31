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
        private readonly IReservingBookRepository _reservationRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationRepository notificationRepository, IReservingBookRepository reservingBookRepository,
            IMapper mapper, ILogger<NotificationService> logger, IBookRepository bookRepository)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _reservationRepository = reservingBookRepository ?? throw new ArgumentNullException(nameof(reservingBookRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
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

        public async Task<IEnumerable<GetNotificationDTO>> GetUserNotificationsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("GetUserNotificationsAsync called with empty userId.");
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
            if (notificationId == Guid.Empty)
            {
                _logger.LogWarning("MarkNotificationAsReadAsync called with empty notificationId.");
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
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("SendBookAvailableNotificationAsync called with empty userId.");
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            if (bookId == Guid.Empty)
            {
                _logger.LogWarning("SendBookAvailableNotificationAsync called with empty bookId.");
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            var book = await _bookRepository.GetByIdAsync(bookId);

            if (book == null)
            {
                _logger.LogInformation($"Book with ID {bookId} not found.");
                return Result<GetNotificationDTO>.Failure(Errors.BookNotFound);
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = bookId,
                Message = $"The book '{book.Title}' you requested is now available.",
                NotificationType = NotificationType.BookAvailable,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            var createdNotification = await _notificationRepository.CreateAsync(notification);

            if (createdNotification == null)
            {
                _logger.LogError($"Failed to create book available notification for user {userId} and book {bookId}");
                return Result<GetNotificationDTO>.Failure(Errors.NotificationCreationFailed);
            }

            _logger.LogInformation($"Successfully sent book available notification for book '{book.Title}' to user {userId}");
            return Result<GetNotificationDTO>.Success(_mapper.Map<GetNotificationDTO>(createdNotification));
        }

        public async Task<Result<GetNotificationDTO>> SendReservationConfirmationAsync(Guid reservationId, Guid userId, Guid bookId)
        {
            if (reservationId == Guid.Empty)
            {
                _logger.LogWarning("SendReservationConfirmationAsync called with empty reservationId.");
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            if (userId == Guid.Empty)
            {
                _logger.LogWarning("SendReservationConfirmationAsync called with empty userId.");
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            if (bookId == Guid.Empty)
            {
                _logger.LogWarning("SendReservationConfirmationAsync called with empty bookId.");
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            if (reservation == null)
            {
                _logger.LogInformation($"Reservation with ID {reservationId} not found.");
                return Result<GetNotificationDTO>.Failure(Errors.ReservationNotFound);
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = bookId,
                Message = $"Your reservation of '{reservation.Book.Title}' has been confirmed.",
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
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("SendBookDueReminder called with empty userId.");
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            if (bookId == Guid.Empty)
            {
                _logger.LogWarning("SendBookDueReminder called with empty bookId.");
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            var book = await _bookRepository.GetByIdAsync(bookId);

            if (book == null)
            {
                _logger.LogInformation($"Book with ID {bookId} not found.");
                return Result<GetNotificationDTO>.Failure(Errors.BookNotFound);
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = bookId,
                Message = $"This is a reminder that your borrowed book '{book.Title}' is due soon.",
                NotificationType = NotificationType.BookDueReminder,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            var createdNotification = await _notificationRepository.CreateAsync(notification);

            if (createdNotification == null)
            {
                _logger.LogError($"Failed to create book due reminder notification for user {userId} and book {bookId}");
                return Result<GetNotificationDTO>.Failure(Errors.NotificationCreationFailed);
            }

            _logger.LogInformation($"Successfully sent book due reminder for book {bookId} to user {userId}");
            return Result<GetNotificationDTO>.Success(_mapper.Map<GetNotificationDTO>(createdNotification));
        }

        public async Task<Result<GetNotificationDTO>> SendNewBookArrival(Guid userId, Guid bookId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("SendNewBookArrival called with empty userId.");
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            if (bookId == Guid.Empty)
            {
                _logger.LogWarning("SendNewBookArrival called with empty bookId.");
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            var book = await _bookRepository.GetByIdAsync(bookId);

            if (book == null)
            {
                _logger.LogInformation($"Book with ID {bookId} not found.");
                return Result<GetNotificationDTO>.Failure(Errors.BookNotFound);
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = bookId,
                Message = $"A new book '{book.Title}' has arrived that you might be interested in.",
                NotificationType = NotificationType.NewBookArrival,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            var createdNotification = await _notificationRepository.CreateAsync(notification);

            if (createdNotification == null)
            {
                _logger.LogError($"Failed to create new book arrival notification for user {userId} and book {bookId}");
                return Result<GetNotificationDTO>.Failure(Errors.NotificationCreationFailed);
            }

            _logger.LogInformation($"Successfully sent new book arrival notification for book {bookId} to user {userId}");
            return Result<GetNotificationDTO>.Success(_mapper.Map<GetNotificationDTO>(createdNotification));
        }

        public async Task<Result<GetNotificationDTO>> SendLibraryCardExpiry(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("SendLibraryCardExpiry called with empty userId.");
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
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("SendGeneralUpdate called with empty userId.");
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Message = "We have some updates for you. Please check your account.",
                NotificationType = NotificationType.GeneralUpdate,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            var createdNotification = await _notificationRepository.CreateAsync(notification);

            if (createdNotification == null)
            {
                _logger.LogError($"Failed to create general update notification for user {userId}");
                return Result<GetNotificationDTO>.Failure(Errors.NotificationCreationFailed);
            }

            _logger.LogInformation($"Successfully sent general update notification to user {userId}");
            return Result<GetNotificationDTO>.Success(_mapper.Map<GetNotificationDTO>(createdNotification));
        }

        public async Task<Result<GetNotificationDTO>> SendSystemAlert(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("SendSystemAlert called with empty userId.");
                return Result<GetNotificationDTO>.Failure(Errors.NullData);
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Message = "There is a system alert that you should be aware of.",
                NotificationType = NotificationType.SystemAlert,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            var createdNotification = await _notificationRepository.CreateAsync(notification);

            if (createdNotification == null)
            {
                _logger.LogError($"Failed to create system alert notification for user {userId}");
                return Result<GetNotificationDTO>.Failure(Errors.NotificationCreationFailed);
            }

            _logger.LogInformation($"Successfully sent system alert notification to user {userId}");
            return Result<GetNotificationDTO>.Success(_mapper.Map<GetNotificationDTO>(createdNotification));
        }
    }
}