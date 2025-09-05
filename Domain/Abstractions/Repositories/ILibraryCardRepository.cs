using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Repositories
{
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
