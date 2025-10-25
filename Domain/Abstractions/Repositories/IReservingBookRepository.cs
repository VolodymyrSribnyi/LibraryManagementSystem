using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Abstractions.Repositories
{
    /// <summary>
    /// Defines a contract for managing book reservations, including creating, retrieving, updating, and canceling
    /// reservations.
    /// </summary>
    /// <remarks>This interface provides methods for handling book reservation operations, such as reserving
    /// a book, retrieving reservations by various criteria, and checking the existence of a reservation.
    /// Implementations of this interface are expected to handle the persistence and retrieval of reservation
    /// data.</remarks>
    public interface IReservingBookRepository
    {
        /// <summary>
        /// Creates a new book reservation in the system.
        /// </summary>
        /// <param name="reservation">The reservation object containing the reservation details.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="Reservation"/> object.</returns>
        Task<Reservation> ReserveBookAsync(Reservation reservation);

        /// <summary>
        /// Marks a book reservation as returned.
        /// </summary>
        /// <param name="id">The unique identifier of the reservation to mark as returned.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the book was successfully returned; otherwise, <see langword="false"/>.</returns>
        Task<bool> ReturnBookAsync(Guid id);

        /// <summary>
        /// Retrieves a reservation by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the reservation to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Reservation"/> object if found; otherwise, <see langword="null"/>.</returns>
        Task<Reservation> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all reservations for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose reservations to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of all <see cref="Reservation"/> objects for the specified user.</returns>
        Task<IEnumerable<Reservation>> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Retrieves all returned reservations for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose returned reservations to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of returned <see cref="Reservation"/> objects for the specified user.</returns>
        Task<IEnumerable<Reservation>> GetReturnedByUserIdAsync(Guid userId);

        /// <summary>
        /// Retrieves all active reservations for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose active reservations to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of active <see cref="Reservation"/> objects for the specified user.</returns>
        Task<IEnumerable<Reservation>> GetActiveByUserIdAsync(Guid userId);

        /// <summary>
        /// Retrieves all reservations in the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of all <see cref="Reservation"/> objects.</returns>
        Task<IEnumerable<Reservation>> GetAllAsync();

        /// <summary>
        /// Retrieves all active reservations in the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of all active <see cref="Reservation"/> objects.</returns>
        Task<IEnumerable<Reservation>> GetActiveReservationsAsync();

        /// <summary>
        /// Checks whether a reservation already exists in the system.
        /// </summary>
        /// <param name="reservation">The reservation object to check for existence.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the reservation exists; otherwise, <see langword="false"/>.</returns>
        Task<bool> IsReservationExists(Reservation reservation);
    }
}
