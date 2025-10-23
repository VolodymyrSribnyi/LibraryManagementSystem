using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Abstractions.Repositories
{
    /// <summary>
    /// Defines a contract for managing book reservations, including creating, retrieving,  updating, and canceling
    /// reservations.
    /// </summary>
    /// <remarks>This interface provides methods for handling book reservation operations, such as  reserving
    /// a book, retrieving reservations by various criteria, and checking the  existence of a reservation.
    /// Implementations of this interface are expected to  handle the persistence and retrieval of reservation
    /// data.</remarks>
    public interface IReservingBookRepository
    {
        Task<Reservation> ReserveBookAsync(Reservation reservation);
        Task<bool> ReturnBookAsync(Guid id);
        Task<Reservation> GetByIdAsync(Guid id);
        Task<IEnumerable<Reservation>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Reservation>> GetReturnedByUserIdAsync(Guid userId);
        Task<IEnumerable<Reservation>> GetActiveByUserIdAsync(Guid userId);
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<IEnumerable<Reservation>> GetActiveReservationsAsync();
        Task<bool> IsReservationExists(Reservation reservation);
    }
}
