using Application.DTOs.Reservations;
using Application.ErrorHandling;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    /// <summary>
    /// Defines a contract for managing book reservation services, including creating, retrieving, and managing
    /// reservations.
    /// </summary>
    /// <remarks>This service provides methods for handling book reservation operations such as reserving books,
    /// returning books, and retrieving reservations by various criteria. Implementations of this interface are
    /// expected to handle the business logic and coordination with the underlying data access layer.</remarks>
    public interface IReservingBookService
    {
        /// <summary>
        /// Creates a new book reservation for a user.
        /// </summary>
        /// <param name="createReservationDTO">The data transfer object containing the information for the new reservation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetReservationDTO"/> representing the created reservation.</returns>
        Task<Result<GetReservationDTO>> ReserveBookAsync(CreateReservationDTO createReservationDTO);

        /// <summary>
        /// Marks a book reservation as returned.
        /// </summary>
        /// <param name="id">The unique identifier of the reservation to mark as returned.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the book was successfully returned; otherwise, <see langword="false"/>.</returns>
        Task<Result> ReturnBookAsync(Guid id);

        /// <summary>
        /// Retrieves a reservation by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the reservation to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetReservationDTO"/> representing the reservation if found; otherwise, <see langword="null"/>.</returns>
        Task<Result<GetReservationDTO>> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all reservations for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose reservations to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of all <see cref="GetReservationDTO"/> objects for the specified user.</returns>
        Task<IEnumerable<GetReservationDTO>> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Retrieves all returned reservations for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose returned reservations to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of returned <see cref="GetReservationDTO"/> objects for the specified user.</returns>
        Task<IEnumerable<GetReservationDTO>> GetReturnedByUserIdAsync(Guid userId);

        /// <summary>
        /// Retrieves all active reservations for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose active reservations to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of active <see cref="GetReservationDTO"/> objects for the specified user.</returns>
        Task<IEnumerable<GetReservationDTO>> GetActiveByUserIdAsync(Guid userId);

        /// <summary>
        /// Retrieves all reservations in the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of all <see cref="GetReservationDTO"/> objects.</returns>
        Task<IEnumerable<GetReservationDTO>> GetAllAsync();

        /// <summary>
        /// Retrieves all active reservations in the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of all active <see cref="GetReservationDTO"/> objects.</returns>
        Task<IEnumerable<GetReservationDTO>> GetActiveReservationsAsync();
    }
}
