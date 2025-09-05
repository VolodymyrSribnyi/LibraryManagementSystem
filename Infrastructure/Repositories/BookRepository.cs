using Abstractions.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> DeleteAsync(Guid id)
        {
            var bookToDelete = await _libraryContext.Books.FindAsync(id);

            if (bookToDelete == null)
                throw new Exception();

            bookToDelete.IsDeleted = true;
            var deletedBook = _libraryContext.Books.Update(bookToDelete).Entity;

            if (deletedBook == null) throw new Exception();

            await _libraryContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            _libraryContext.Books.AsNoTracking();

            return await _libraryContext.Books.Where(b => b.IsDeleted == false).ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetAllByAuthorAsync(Author author)
        {
            _libraryContext.Books.AsNoTracking();

            return await _libraryContext.Books.Where(b => b.AuthorId == author.Id && b.IsDeleted == false).ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetAllByGenresAsync(IEnumerable<Genre> genres)
        {
            _libraryContext.Books.AsNoTracking();

            return await _libraryContext.Books.Where(b => genres.Contains(b.Genre) && b.IsDeleted == false).ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetAllByPublisherAsync(IEnumerable<string> publishers)
        {
            _libraryContext.Books.AsNoTracking();

            return await _libraryContext.Books.Where(b => publishers.Contains(b.Publisher) && b.IsDeleted == false).ToListAsync();
        }

        public async Task<Book> GetByIdAsync(Guid id)
        {
            var book = await _libraryContext.Books.FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted == false);

            return book;
        }

        public async Task<Book> GetByTitleAsync(string title)
        {
            _libraryContext.Books.AsNoTracking();

            var book = await _libraryContext.Books.FirstOrDefaultAsync(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase) && b.IsDeleted == false);

            return book;
        }

        public async Task<Book> UpdateAsync(Book book)
        {
            var bookToUpdate = await _libraryContext.Books.FindAsync(book.Id);

            bookToUpdate.Id = book.Id;
            bookToUpdate.Title = book.Title;
            bookToUpdate.AuthorId = book.AuthorId;
            bookToUpdate.Genre = book.Genre;
            bookToUpdate.Publisher = book.Publisher;
            bookToUpdate.PublishingYear = book.PublishingYear;
            bookToUpdate.IsAvailable = book.IsAvailable;
            bookToUpdate.Rating = book.Rating;
            bookToUpdate.IsDeleted = book.IsDeleted;
            bookToUpdate.CreatedAt = book.CreatedAt;
            bookToUpdate.LastUpdatedAt = DateTime.UtcNow;

            await _libraryContext.SaveChangesAsync();

            return bookToUpdate;
        }

        public async Task<bool> UpdateAvailabilityAsync(Book book)
        {
            var bookToUpdate = await _libraryContext.Books.FindAsync(book.Id);

            bookToUpdate.IsAvailable = book.IsAvailable;

            await _libraryContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateRatingAsync(Book book)
        {
            var bookToUpdate = await _libraryContext.Books.FindAsync(book.Id);

            bookToUpdate.Rating = book.Rating;

            await _libraryContext.SaveChangesAsync();

            return true;
        }
    }
}
