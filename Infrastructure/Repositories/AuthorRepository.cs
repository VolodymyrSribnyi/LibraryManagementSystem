using Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibraryContext _libraryContext;
        public AuthorRepository(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }
        public async Task<Author> AddAsync(Author author)
        {
            var createdAuthor = _libraryContext.Authors.Add(author).Entity;

            await _libraryContext.SaveChangesAsync();

            return createdAuthor;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var authorToDelete = await _libraryContext.Authors.FirstOrDefaultAsync(a => a.Id == id);

            authorToDelete.IsDeleted = true;
            var deletedAuthor = _libraryContext.Authors.Update(authorToDelete).Entity;  

            await _libraryContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            _libraryContext.Authors.AsNoTracking();
            return await _libraryContext.Authors.Where(a => a.IsDeleted == false).ToListAsync();
        }

        public async Task<Author> GetByFullNameAsync(string fullName)
        {
            string[] parts = fullName.Split(' ');
            var firstName = parts[0];
            var lastName = parts[1];

            var author = await _libraryContext.Authors.
                FirstOrDefaultAsync(a => a.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) 
                && a.Surname.Equals(lastName, StringComparison.OrdinalIgnoreCase));

            return author;
        }

        public async Task<Author> GetByIdAsync(Guid id)
        {
            var author = await _libraryContext.Authors.FindAsync(id);

            return author;
        }

        public async Task<Author> UpdateAsync(Author author)
        {
            var authorToUpdate = await _libraryContext.Authors.FindAsync(author.Id);

            authorToUpdate.Id = author.Id;
            authorToUpdate.FirstName = author.FirstName;
            authorToUpdate.Surname = author.Surname;
            authorToUpdate.Age = author.Age;
            authorToUpdate.IsDeleted = author.IsDeleted;
            authorToUpdate.Books = author.Books;
            authorToUpdate.CreatedAt = author.CreatedAt;
            author.MiddleName = author.MiddleName;

            await _libraryContext.SaveChangesAsync();

            return authorToUpdate;
        }
    }
}
