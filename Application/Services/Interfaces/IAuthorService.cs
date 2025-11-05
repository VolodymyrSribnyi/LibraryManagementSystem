using Application.DTOs.Authors;
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
    /// Defines a contract for managing author-related operations, including creating, updating, retrieving, and
    /// deleting authors.
    /// </summary>
    /// <remarks>This service provides methods for performing CRUD operations on authors, as well as mapping
    /// between data transfer objects. Implementations of this interface are expected to handle the underlying data
    /// access and business logic.</remarks>
    public interface IAuthorService
    {
        /// <summary>
        /// Creates a new author in the system.
        /// </summary>
        /// <param name="createAuthorDTO">The data transfer object containing the information for the new author.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetAuthorDTO"/> representing the created author.</returns>
        Task<Result<GetAuthorDTO>> AddAsync(CreateAuthorDTO createAuthorDTO);

        /// <summary>
        /// Updates an existing author's information.
        /// </summary>
        /// <param name="updateAuthorDTO">The data transfer object containing the updated author information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetAuthorDTO"/> representing the updated author.</returns>
        Task<Result<GetAuthorDTO>> UpdateAsync(UpdateAuthorDTO updateAuthorDTO);

        /// <summary>
        /// Deletes an author from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the author to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the author was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<Result<bool>> DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves an author by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the author to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetAuthorDTO"/> representing the author if found; otherwise, <see langword="null"/>.</returns>
        Task<Result<GetAuthorDTO>> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves an author by their full name.
        /// </summary>
        /// <param name="fullName">The full name of the author to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetAuthorDTO"/> representing the author if found; otherwise, <see langword="null"/>.</returns>
        Task<Result<GetAuthorDTO>> GetBySurnameAsync(string fullName);

        /// <summary>
        /// Retrieves all authors in the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="GetAuthorDTO"/> objects representing all authors.</returns>
        Task<Result<IEnumerable<GetAuthorDTO>>> GetAllAsync();

        /// <summary>
        /// Maps a <see cref="GetAuthorDTO"/> object to an <see cref="UpdateAuthorDTO"/> object.
        /// </summary>
        /// <param name="getAuthorDTO">The author data transfer object to map.</param>
        /// <returns>An <see cref="UpdateAuthorDTO"/> object populated with data from the provided <see cref="GetAuthorDTO"/>.</returns>
        UpdateAuthorDTO MapToUpdateAuthorDTO(GetAuthorDTO getAuthorDTO);
    }
}
