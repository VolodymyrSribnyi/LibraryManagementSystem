using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class LibraryCardRepository : ILibraryCardRepository
    {
        public Task<LibraryCard> CreateAsync(LibraryCard libraryCard)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LibraryCard>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<LibraryCard> GetByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<DateTime?> GetExpirationDateAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsActiveAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsExistsAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsExpiredAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<LibraryCard> UpdateAsync(LibraryCard libraryCard)
        {
            throw new NotImplementedException();
        }
    }
}
