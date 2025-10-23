using Abstractions.Repositories;
using Application.DTOs.Notitfications;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IReservingBookRepository _reservationRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;
        public NotificationService(INotificationRepository notificationRepository, IReservingBookRepository reservingBookRepository,IMapper mapper,
            ILogger<NotificationService> logger,IBookRepository bookRepository)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _reservationRepository = reservingBookRepository ?? throw new ArgumentNullException(nameof(reservingBookRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException( nameof(logger));
            _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        }
        public async Task<GetNotificationDTO> CreateNotification(CreateNotificationDTO createNotificationDTO)
        {
            if(createNotificationDTO == null)
                throw new ArgumentNullException(nameof(createNotificationDTO));

            var notificationToCreate = _mapper.Map<Notification>(createNotificationDTO);

            var notification = await _notificationRepository.CreateAsync(notificationToCreate);

            if (notification == null)
            {
                throw new InvalidOperationException($"Failed to create notification of type {createNotificationDTO.NotificationType}" );
            }

            _logger.LogInformation("Successfully created notification");
            return _mapper.Map<GetNotificationDTO>(notification);
        }

        public async Task<IEnumerable<GetNotificationDTO>> GetUserNotificationsAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

            var notifications = await _notificationRepository.GetUserNotificationsAsync(userId);

            if (notifications == null || !notifications.Any())
            {
                _logger.LogInformation($"No notifications found for user with ID: {userId}");
                return Enumerable.Empty<GetNotificationDTO>();
            }

            return _mapper.Map<IEnumerable<GetNotificationDTO>>(notifications);
        }

        public async Task<bool> MarkNotificationAsReadAsync(Guid notificationId)
        {
            if (notificationId == Guid.Empty)
                throw new ArgumentNullException(nameof(notificationId));

            var notification = await _notificationRepository.GetByIdAsync(notificationId);

            if (notification == null)
            {
                throw new NotificationNotFoundException($"Notification with id {notificationId} not found");
            }

            var success = await _notificationRepository.MarkNotificationAsReadAsync(notification.Id);

            if (!success)
            {
                throw new InvalidOperationException("Failed to mark notification as read");
            }

            return true;
        }

        public async Task<GetNotificationDTO> SendBookAvailableNotificationAsync(Guid userId, Guid bookId)
        {
            if (userId == Guid.Empty || bookId == Guid.Empty)
                throw new ArgumentNullException("userId or bookId is empty");

            var book = await  _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                throw new BookNotFoundException(bookId);

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = bookId,
                Message = $"The book with title {book.Title} you requested is now available.",
                NotificationType = NotificationType.BookAvailable,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            var createdNotification = await _notificationRepository.CreateAsync(notification);

            if (createdNotification == null)
            {
                throw new InvalidOperationException("Failed to create book available notification");
            }

            return _mapper.Map<GetNotificationDTO>(createdNotification);
        }

        public async Task<GetNotificationDTO> SendReservationConfirmationAsync(Guid reservationId, Guid userId, Guid bookId)
        {
            if (reservationId == Guid.Empty || userId == Guid.Empty || bookId == Guid.Empty)
                throw new ArgumentNullException("reservationId, userId or bookId is empty");

            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            if (reservation == null)
            {
                throw new Exception("Reservation not found");
            }

            var notification = new Notification
            {
                Id = reservationId,
                UserId = userId,
                BookId = bookId,
                Message = $"Your reservation of {reservation.Book.Title} has been confirmed.",
                NotificationType = NotificationType.ReservationConfirmation,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            var createdNotification = await _notificationRepository.CreateAsync(notification);

            if (createdNotification == null)
            {
                throw new InvalidOperationException("Failed to create reservation confirmation notification");
            }

            return _mapper.Map<GetNotificationDTO>(createdNotification);
        }
        public async Task<GetNotificationDTO> SendBookDueReminder(Guid userId, Guid bookId)
        {
            if (userId == Guid.Empty || bookId == Guid.Empty)
                throw new ArgumentNullException("userId or bookId is empty");

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = bookId,
                Message = "This is a reminder that your borrowed book is due soon.",
                NotificationType = NotificationType.BookDueReminder,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            var createdNotification = await _notificationRepository.CreateAsync(notification);

            if (createdNotification == null)
            {
                throw new InvalidOperationException("Failed to create book due reminder notification");
            }

            return _mapper.Map<GetNotificationDTO>(createdNotification);
        }
        public async Task<GetNotificationDTO> SendNewBookArrival(Guid userId, Guid bookId)
        {
            if (userId == Guid.Empty || bookId == Guid.Empty)
                throw new ArgumentNullException("userId or bookId is empty");

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = bookId,
                Message = "A new book has arrived that you might be interested in.",
                NotificationType = NotificationType.NewBookArrival,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            var createdNotification = await _notificationRepository.CreateAsync(notification);

            if (createdNotification == null)
            {
                throw new InvalidOperationException("Failed to create new book arrival notification");
            }

            return _mapper.Map<GetNotificationDTO>(createdNotification);
        }
        public async Task<GetNotificationDTO> SendLibraryCardExpiry(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

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
                throw new InvalidOperationException("Failed to create library card expiry notification");
            }

            return _mapper.Map<GetNotificationDTO>(createdNotification);
        }
        public async Task<GetNotificationDTO> SendGeneralUpdate(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

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
                throw new InvalidOperationException("Failed to create general update notification");
            }

            return _mapper.Map<GetNotificationDTO>(createdNotification);
        }
        public async Task<GetNotificationDTO> SendSystemAlert(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId));

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
                throw new InvalidOperationException("Failed to create system alert notification");
            }

            return _mapper.Map<GetNotificationDTO>(createdNotification);
        }
    }
}
