using Application.DTOs.Reservations;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IReservingBookService
    {
        Task<GetReservationDTO> ReserveBookAsync(CreateReservationDTO createReservationDTO);
        /// <summary>
        /// Cancels a reservation by its ID.
        /// </summary>
        /// <param name="id">The ID of the reservation to cancel.</param>
        /// <returns>True if the cancellation was successful, otherwise false.</returns>
        Task<bool> ReturnBookAsync(Guid id);
        Task<GetReservationDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<GetReservationDTO>> GetByUserIdAsync(Guid userId);
        /// <summary>
        /// Retrieves all reservations.
        /// </summary>
        /// <returns>A list of all reservations.</returns>
        Task<IEnumerable<GetReservationDTO>> GetAllAsync();
        Task<IEnumerable<GetReservationDTO>> GetActiveReservationsAsync();
    }
}
