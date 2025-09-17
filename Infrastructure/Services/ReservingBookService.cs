using Abstractions.Repositories;
using Application.DTOs.Reservations;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ReservingBookService : IReservingBookService
    {
        private readonly IReservingBookRepository _reservingBookRepository;
        private readonly IBookRepository _bookRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<ReservingBookService> _logger;
        public ReservingBookService(IReservingBookRepository reservingBookRepository,IMapper mapper, IBookRepository bookRepository,
            UserManager<ApplicationUser> userManager,ILogger<ReservingBookService> logger)
        {
            _reservingBookRepository = reservingBookRepository;
            _mapper = mapper;
            _bookRepository = bookRepository;
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<GetReservationDTO> ReserveBookAsync(CreateReservationDTO createReservationDTO)
        {
            if (createReservationDTO == null)
                throw new ArgumentNullException(nameof(createReservationDTO));

            var reservationToCreate = _mapper.Map<Reservation>(createReservationDTO);
            var existingReservation = await _reservingBookRepository.IsReservationExists(reservationToCreate);

            if (existingReservation == true)
            {
                throw new ReservationExistsException($"Reservation with for userId {reservationToCreate.UserId} and bookId {reservationToCreate.BookId} already exists.");
            }

            var bookToReserve = await _bookRepository.GetByIdAsync(reservationToCreate.BookId);

            if (bookToReserve == null)
            {
                throw new BookNotFoundException($"Book with id {reservationToCreate.BookId} not found");
            }

            var user = await _userManager.FindByIdAsync(reservationToCreate.UserId.ToString());

            if (user == null)
            {
                throw new UserNotFoundException($"User with id {reservationToCreate.UserId} not found");
            }

            var reservation = await _reservingBookRepository.ReserveBookAsync(reservationToCreate);

            if (reservation == null)
            {
                throw new InvalidOperationException("Reservation could not be created.");
            }

            _logger.LogInformation("Reservation successfully created");
            return _mapper.Map<GetReservationDTO>(reservation);
        }

        public async Task<bool> ReturnBookAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id), "Reservation ID cannot be empty.");

            var reservationToCancel = await _reservingBookRepository.GetByIdAsync(id);

            if (reservationToCancel == null)
            {
                throw new Exception("Reservation not found.");
            }

            var result = await _reservingBookRepository.ReturnBookAsync(id);

            if (!result)
            {
                throw new ReservationNotFoundException($"Reservation with id {reservationToCancel.Id} not found");
            }

            _logger.LogInformation("Reservation successfully canceled");
            return true;
        }

        public async Task<GetReservationDTO> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id), "Reservation ID cannot be empty.");

            var reservation = await _reservingBookRepository.GetByIdAsync(id);

            if (reservation == null)
            {
                throw new ReservationNotFoundException($"Reservation with id {reservation.Id} not found");
            }

            return _mapper.Map<GetReservationDTO>(reservation);
        }

        public async Task<IEnumerable<GetReservationDTO>> GetByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId), "Reservation ID cannot be empty.");

            var reservations = await _reservingBookRepository.GetByUserIdAsync(userId);

            if (reservations == null || !reservations.Any())
            {
                _logger.LogInformation("No reservations found for this user.");
                return Enumerable.Empty<GetReservationDTO>();
            }

            return _mapper.Map<IEnumerable<GetReservationDTO>>(reservations);
        }

        public async Task<IEnumerable<GetReservationDTO>> GetAllAsync()
        {
            var reservations = await _reservingBookRepository.GetAllAsync();

            if (reservations == null || !reservations.Any())
            {
                _logger.LogInformation("No reservations found.");
                return Enumerable.Empty<GetReservationDTO>();
            }

            return _mapper.Map<IEnumerable<GetReservationDTO>>(reservations);
        }

        public async Task<IEnumerable<GetReservationDTO>> GetActiveReservationsAsync()
        {
            var reservations = await _reservingBookRepository.GetActiveReservationsAsync();

            if (reservations == null || !reservations.Any())
            {
                _logger.LogInformation("No reservations found.");
                return Enumerable.Empty<GetReservationDTO>();
            }

            return _mapper.Map<IEnumerable<GetReservationDTO>>(reservations);
        }
    }
}
