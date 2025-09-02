using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryContext _libraryContext;
        public BookRepository(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }

        public async Task<Book> AddAsync(Book book)
        {
            var createdBook = _libraryContext.Books.Add(book).Entity;

            await _libraryContext.SaveChangesAsync();

            return createdBook;
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Book>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Book>> GetAllByAuthorAsync(Author author)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Book>> GetAllByGenresAsync(IEnumerable<Genre> genres)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Book>> GetAllByPublisherAsync(IEnumerable<string> publishers)
        {
            throw new NotImplementedException();
        }

        public Task<Book> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Book> GetByTitleAsync(string title)
        {
            throw new NotImplementedException();
        }

        public Task<Book> UpdateAsync(Book book)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAvailabilityAsync(Book book)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRatingAsync(Book book)
        {
            throw new NotImplementedException();
        }
    }
}
