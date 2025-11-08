using Abstractions.Repositories;
using Application.DTOs.Reservations;
using Application.ErrorHandling;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Events;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class ReservingBookService : IReservingBookService
    {
        private readonly IReservingBookRepository _reservingBookRepository;
        private readonly IBookRepository _bookRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IDomainEventPublisher _domainEventPublisher;
        private readonly IMapper _mapper;
        private readonly ILogger<ReservingBookService> _logger;
        private readonly ILibraryCardService _libraryCardService;
        public ReservingBookService(IReservingBookRepository reservingBookRepository, IMapper mapper, IBookRepository bookRepository,
            UserManager<ApplicationUser> userManager, ILogger<ReservingBookService> logger, IDomainEventPublisher domainEventPublisher,
            INotificationService notificationService,ILibraryCardService libraryCardService)
        {
            _reservingBookRepository = reservingBookRepository;
            _mapper = mapper;
            _bookRepository = bookRepository;
            _userManager = userManager;
            _domainEventPublisher = domainEventPublisher;
            _logger = logger;
            _notificationService = notificationService;
            _libraryCardService = libraryCardService;
        }
        public async Task<Result<GetReservationDTO>> ReserveBookAsync(CreateReservationDTO createReservationDTO)
        {
            if (createReservationDTO == null)
            {
                _logger.LogWarning("ReserveBookAsync called with null CreateReservationDTO.");
                return Result<GetReservationDTO>.Failure(Errors.NullData);
            }

            var reservationToCreate = _mapper.Map<Reservation>(createReservationDTO);
            var existingReservation = await _reservingBookRepository.IsReservationExists(reservationToCreate);

            if (existingReservation)
            {
                _logger.LogInformation($"Reservation already exists for userId {reservationToCreate.UserId} and bookId {reservationToCreate.BookId}");
                return Result<GetReservationDTO>.Failure(Errors.ReservationExists);
            }

            var bookToReserve = await _bookRepository.GetByIdAsync(reservationToCreate.BookId);

            if (bookToReserve == null)
            {
                _logger.LogInformation($"Book with ID {reservationToCreate.BookId} not found.");
                return Result<GetReservationDTO>.Failure(Errors.BookNotFound);
            }

            var user = await _userManager.FindByIdAsync(reservationToCreate.UserId.ToString());

            if (user == null)
            {
                _logger.LogInformation($"User with ID {reservationToCreate.UserId} not found.");
                return Result<GetReservationDTO>.Failure(Errors.UserNotFound);
            }

            var librarycardExists = await _libraryCardService.IsExistsAsync(reservationToCreate.UserId);

            if (!librarycardExists.Value)
            {
                _logger.LogInformation($"Library card for user ID {reservationToCreate.UserId} not found.");
                return Result<GetReservationDTO>.Failure(Errors.LibraryCardNotFound);
            }

            var reservation = await _reservingBookRepository.ReserveBookAsync(reservationToCreate);

            if (reservation == null)
            {
                _logger.LogError($"Failed to create reservation for user {reservationToCreate.UserId} and book {reservationToCreate.BookId}");
                return Result<GetReservationDTO>.Failure(Errors.ReservationCreationFailed);
            }

            await _notificationService.SendReservationConfirmationAsync(reservation.Id, user.Id, bookToReserve.Id);

            _logger.LogInformation($"Successfully created reservation with ID: {reservation.Id}");
            return Result<GetReservationDTO>.Success(_mapper.Map<GetReservationDTO>(reservation));
        }
        public async Task<Result> ReturnBookAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("ReturnBookAsync called with empty reservation ID.");
                return Result.Failure(Errors.NullData);
            }

            var reservationToCancel = await _reservingBookRepository.GetByIdAsync(id);

            if (reservationToCancel == null)
            {
                _logger.LogInformation($"Reservation with ID {id} not found.");
                return Result.Failure(Errors.ReservationNotFound);
            }

            reservationToCancel.Book = await _bookRepository.GetByIdAsync(reservationToCancel.BookId);

            if (reservationToCancel.Book == null)
            {
                _logger.LogWarning($"Book with ID {reservationToCancel.BookId} not found for reservation {id}.");
                return Result.Failure(Errors.BookNotFound);
            }

            var result = await _reservingBookRepository.ReturnBookAsync(id);

            if (!result)
            {
                _logger.LogError($"Failed to return book for reservation with ID {id}");
                return Result.Failure(Errors.FailedToReturnBook);
            }

            var domainEvent = new BookBecameAvailableEvent(reservationToCancel.BookId, reservationToCancel.Book.Title);
            await _domainEventPublisher.PublishAsync(domainEvent);

            _logger.LogInformation($"Successfully returned book for reservation with ID: {id}");
            return Result.Success();
        }

        public async Task<Result<GetReservationDTO>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("GetByIdAsync called with empty reservation ID.");
                return Result<GetReservationDTO>.Failure(Errors.NullData);
            }

            var reservation = await _reservingBookRepository.GetByIdAsync(id);

            if (reservation == null)
            {
                _logger.LogInformation($"Reservation with ID {id} not found.");
                return Result<GetReservationDTO>.Failure(Errors.ReservationNotFound);
            }

            return Result<GetReservationDTO>.Success(_mapper.Map<GetReservationDTO>(reservation));
        }
        public async Task<IEnumerable<GetReservationDTO>> GetReturnedByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("GetReturnedByUserIdAsync called with empty userId.");
                return Enumerable.Empty<GetReservationDTO>();
            }

            var reservations = await _reservingBookRepository.GetReturnedByUserIdAsync(userId);

            if (reservations == null || !reservations.Any())
            {
                _logger.LogInformation($"No returned reservations found for user ID: {userId}");
                return Enumerable.Empty<GetReservationDTO>();
            }

            var reservationsDTO = _mapper.Map<IEnumerable<GetReservationDTO>>(reservations);

           
            _logger.LogInformation($"Retrieved {reservations.Count()} returned reservations for user ID: {userId}");
            return _mapper.Map<IEnumerable<GetReservationDTO>>(reservations);
        }
        public async Task<IEnumerable<GetReservationDTO>> GetByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("GetByUserIdAsync called with empty userId.");
                return Enumerable.Empty<GetReservationDTO>();
            }

            var reservations = await _reservingBookRepository.GetByUserIdAsync(userId);

            if (reservations == null || !reservations.Any())
            {
                _logger.LogInformation($"No reservations found for user ID: {userId}");
                return Enumerable.Empty<GetReservationDTO>();
            }

            _logger.LogInformation($"Retrieved {reservations.Count()} reservations for user ID: {userId}");
            return _mapper.Map<IEnumerable<GetReservationDTO>>(reservations);
        }
        public async Task<IEnumerable<GetReservationDTO>> GetActiveByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("GetActiveByUserIdAsync called with empty userId.");
                return Enumerable.Empty<GetReservationDTO>();
            }

            var reservations = await _reservingBookRepository.GetActiveByUserIdAsync(userId);

            if (reservations == null || !reservations.Any())
            {
                _logger.LogInformation($"No active reservations found for user ID: {userId}");
                return Enumerable.Empty<GetReservationDTO>();
            }

            _logger.LogInformation($"Retrieved {reservations.Count()} active reservations for user ID: {userId}");
            return _mapper.Map<IEnumerable<GetReservationDTO>>(reservations);
        }

        public async Task<IEnumerable<GetReservationDTO>> GetAllAsync()
        {
            var reservations = await _reservingBookRepository.GetAllAsync();

            if (reservations == null || !reservations.Any())
            {
                _logger.LogInformation("No reservations found in the system.");
                return Enumerable.Empty<GetReservationDTO>();
            }

            _logger.LogInformation($"Retrieved {reservations.Count()} total reservations.");
            return _mapper.Map<IEnumerable<GetReservationDTO>>(reservations);
        }

        public async Task<IEnumerable<GetReservationDTO>> GetActiveReservationsAsync()
        {
            var reservations = await _reservingBookRepository.GetActiveReservationsAsync();

            if (reservations == null || !reservations.Any())
            {
                _logger.LogInformation("No active reservations found in the system.");
                return Enumerable.Empty<GetReservationDTO>();
            }

            _logger.LogInformation($"Retrieved {reservations.Count()} active reservations.");
            return _mapper.Map<IEnumerable<GetReservationDTO>>(reservations);
        }
    }
}
