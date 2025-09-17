using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//+ReserveBook(IBook book): IReservation
//+ CancelReservation(Guid id): bool
//+ GetAll() : List < IReservation >
namespace Abstractions.Repositories
{
    public interface IReservingBookRepository
    {
        Task<Reservation> ReserveBookAsync(Reservation reservation);
        /// <summary>
        /// Cancels a reservation by its ID.
        /// </summary>
        /// <param name="id">The ID of the reservation to cancel.</param>
        /// <returns>True if the cancellation was successful, otherwise false.</returns>
        Task<bool> ReturnBookAsync(Guid id);
        Task<Reservation> GetByIdAsync(Guid id);
        Task<IEnumerable<Reservation>> GetByUserIdAsync(Guid userId);
        /// <summary>
        /// Retrieves all reservations.
        /// </summary>
        /// <returns>A list of all reservations.</returns>
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<IEnumerable<Reservation>> GetActiveReservationsAsync();
        Task<bool> IsReservationExists(Reservation reservation);
    }
}
