using Abstractions.Repositories;
using Application.DTOs.Reservations;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
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
        private readonly IMapper _mapper;
        public ReservingBookService(IReservingBookRepository reservingBookRepository,IMapper mapper)
        {
            _reservingBookRepository = reservingBookRepository;
            _mapper = mapper;
        }
        public async Task<GetReservationDTO> ReserveBookAsync(CreateReservationDTO createReservationDTO)
        {
            var reservationToCreate = _mapper.Map<Reservation>(createReservationDTO);

            var reservation = await _reservingBookRepository.ReserveBookAsync(reservationToCreate);

            if (reservation == null)
            {
                throw new Exception("Reservation could not be created.");
            }

            return _mapper.Map<GetReservationDTO>(reservation);
        }

        public async Task<GetReservationDTO> UpdateStatusAsync(UpdateReservationStatusDTO updateReservationStatusDTO)
        {
            var reservationExists = await _reservingBookRepository.GetByIdAsync(updateReservationStatusDTO.Id);

            if (reservationExists == null)
                throw new Exception();

            _mapper.Map(updateReservationStatusDTO, reservationExists);

            var updatedReservation = await _reservingBookRepository.UpdateStatusAsync(reservationExists);

            if (updatedReservation == null)
                throw new Exception();

            return _mapper.Map<GetReservationDTO>(updatedReservation);
        }

        public async Task<bool> CancelReservationAsync(Guid id)
        {
            var reservationToCancel = await _reservingBookRepository.GetByIdAsync(id);

            if (reservationToCancel == null)
            {
                throw new Exception("Reservation not found.");
            }

            var result = await _reservingBookRepository.CancelReservationAsync(id);

            if (!result)
            {
                throw new Exception("Failed to cancel reservation.");
            }

            return true;
        }

        public async Task<GetReservationDTO> GetByIdAsync(Guid id)
        {
            var reservation = await _reservingBookRepository.GetByIdAsync(id);

            if (reservation == null)
            {
                throw new Exception("Reservation not found.");
            }

            return _mapper.Map<GetReservationDTO>(reservation);
        }

        public async Task<IEnumerable<GetReservationDTO>> GetByUserIdAsync(Guid userId)
        {
            var reservations = await _reservingBookRepository.GetByUserIdAsync(userId);

            if (reservations == null || !reservations.Any())
            {
                throw new Exception("No reservations found for this user.");
            }

            return _mapper.Map<IEnumerable<GetReservationDTO>>(reservations);
        }

        public async Task<IEnumerable<GetReservationDTO>> GetAllAsync()
        {
            var reservations = await _reservingBookRepository.GetAllAsync();

            if (reservations == null || !reservations.Any())
            {
                throw new Exception("No reservations found.");
            }

            return _mapper.Map<IEnumerable<GetReservationDTO>>(reservations);
        }

        public async Task<IEnumerable<GetReservationDTO>> GetActiveReservationsAsync()
        {
            var reservations = await _reservingBookRepository.GetActiveReservationsAsync();

            if (reservations == null || !reservations.Any())
            {
                throw new Exception("No active reservations found.");
            }

            return _mapper.Map<IEnumerable<GetReservationDTO>>(reservations);
        }
    }
}
