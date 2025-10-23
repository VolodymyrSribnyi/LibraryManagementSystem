using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Repositories
{
    /// <summary>
    /// Defines a contract for managing library card data, including creation, updates, retrieval, and validation.
    /// </summary>
    /// <remarks>This interface provides methods for performing CRUD operations on library cards, as well as
    /// utility methods for checking the existence, status, and expiration of a library card associated with a specific
    /// user. Implementations of this interface are expected to handle data persistence and retrieval.</remarks>
    public interface ILibraryCardRepository
    {
        Task<LibraryCard> CreateAsync(LibraryCard libraryCard);
        Task<LibraryCard> UpdateAsync(LibraryCard libraryCard);
        Task<bool> DeleteAsync(Guid userId);
        Task<LibraryCard> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<LibraryCard>> GetAllAsync();
        Task<bool> IsExistsAsync(Guid userId);
        Task<bool> IsActiveAsync(Guid userId);
        Task<DateTime?> GetExpirationDateAsync(Guid userId);
    }
}
