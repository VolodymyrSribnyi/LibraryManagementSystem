using Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class LibraryCardRepository : ILibraryCardRepository
    {
        private readonly LibraryContext _libraryContext;
        public LibraryCardRepository(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }
        public async Task<LibraryCard> CreateAsync(LibraryCard libraryCard)
        {
            if (IsExistsAsync(libraryCard.UserId).Result)
                throw new Exception("Library card already exists for this user.");

            var createdLibraryCard =  _libraryContext.LibraryCards.Add(libraryCard).Entity;

            await _libraryContext.SaveChangesAsync();

            return createdLibraryCard;
        }

        public async Task<bool> DeleteAsync(Guid userId)
        {
            var libraryCardToDelete = await _libraryContext.LibraryCards.FirstOrDefaultAsync(lc => lc.UserId == userId);

            libraryCardToDelete.IsDeleted = true;
            var deletedLibraryCard = _libraryContext.LibraryCards.Update(libraryCardToDelete).Entity;

            await _libraryContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<LibraryCard>> GetAllAsync()
        {
            _libraryContext.LibraryCards.AsNoTracking();

            return await _libraryContext.LibraryCards.Where(lc => lc.IsDeleted == false).ToListAsync();
        }

        public async Task<LibraryCard> GetByUserIdAsync(Guid userId)
        {
            var libraryCard = await _libraryContext.LibraryCards.FirstOrDefaultAsync(lc => lc.UserId == userId && lc.IsDeleted == false);

            return libraryCard;
        }

        public async Task<DateTime?> GetExpirationDateAsync(Guid userId)
        {
            var expirationDate = await _libraryContext.LibraryCards
                .Where(lc => lc.UserId == userId && lc.IsDeleted == false)
                .Select(lc => (DateTime?)lc.ValidTo)
                .FirstOrDefaultAsync();

            return expirationDate;
        }

        public async Task<bool> IsActiveAsync(Guid userId)
        {
            var isActive = await _libraryContext.LibraryCards.AnyAsync(lc => lc.UserId == userId && lc.IsValid && !lc.IsDeleted && lc.ValidTo > DateTime.UtcNow);

            return isActive;
        }

        public async Task<bool> IsExistsAsync(Guid userId)
        {
            var isExists = await _libraryContext.LibraryCards.AnyAsync(lc => lc.UserId == userId && !lc.IsDeleted);

            return isExists;
        }

        public async Task<LibraryCard> UpdateAsync(LibraryCard libraryCard)
        {
            var libraryCardToUpdate = await _libraryContext.LibraryCards.FindAsync(libraryCard.Id);

            libraryCardToUpdate.Id = libraryCard.Id;
            libraryCardToUpdate.UserId = libraryCard.UserId;
            libraryCardToUpdate.ValidTo = libraryCard.ValidTo;
            libraryCardToUpdate.IsValid = libraryCard.IsValid;
            libraryCardToUpdate.IsDeleted = libraryCard.IsDeleted;

            await _libraryContext.SaveChangesAsync();

            return libraryCardToUpdate;
        }
    }
}
