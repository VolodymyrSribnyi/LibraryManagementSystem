using Abstractions.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        public async Task<Author> Get(Expression<Func<Author, bool>> filter, string? includeProperties = null)
        {
            IQueryable<Author> query = _libraryContext.Set<Author>();

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Author>> GetAll(Expression<Func<Author, bool>>? filter = null, string? includeProperties = null)  
        {
            IQueryable<Author> query = _libraryContext.Set<Author>();

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            _libraryContext.Authors.AsNoTracking();
            return await GetAll(a => a.IsDeleted == false,"Books");
        }

        public async Task<Author> GetByFullNameAsync(string fullName)
        {
            string[] parts = fullName.Split(' ');
            var firstName = parts[0];
            var lastName = parts[1];

            var author = await Get(a => a.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) 
                && a.Surname.Equals(lastName, StringComparison.OrdinalIgnoreCase));

            return author;
        }

        public async Task<Author> GetByIdAsync(Guid id)
        {
            var author = await Get(a => a.Id == id,"Books");

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
