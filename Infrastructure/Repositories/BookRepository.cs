using Abstractions.Repositories;
using Application.Filters;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

            bookToDelete.IsDeleted = true;

            var deletedBook = _libraryContext.Books.Update(bookToDelete).Entity;

            await _libraryContext.SaveChangesAsync();

            return true;
        }
        public async Task<IEnumerable<Book>> GetFilteredAsync(Expression<Func<Book,bool>> filter)
        {
            var query = _libraryContext.Books
                .AsNoTracking()
                .Include(b => b.Author)
                .Where(filter);

            return await query.ToListAsync();
        }
        public async Task<Book> Get(Expression<Func<Book, bool>> filter, string? includeProperties = null)
        {
            IQueryable<Book> query = _libraryContext.Set<Book>();

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
        public async Task<IEnumerable<Book>> GetAll(Expression<Func<Book, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<Book> query = _libraryContext.Set<Book>();

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

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            _libraryContext.Books.AsNoTracking();

            var books = await GetAll(b => b.IsDeleted == false);

            foreach (var book in books)
            {
                book.Author = await _libraryContext.Authors.FirstOrDefaultAsync(b => b.Id == book.AuthorId);
            }

            return books;
        }

        public async Task<IEnumerable<Book>> GetAllByAuthorAsync(Author author)
        {
            _libraryContext.Books.AsNoTracking();

            return await GetAll(b => b.AuthorId == author.Id && b.IsDeleted == false);
        }

        public async Task<IEnumerable<Book>> GetAllByGenresAsync(IEnumerable<Genre> genres)
        {
            _libraryContext.Books.AsNoTracking();

            return await GetAll(b => genres.Contains(b.Genre) && b.IsDeleted == false);
        }

        public async Task<IEnumerable<Book>> GetAllByPublisherAsync(IEnumerable<string> publishers)
        {
            _libraryContext.Books.AsNoTracking();

            return await GetAll(b => publishers.Contains(b.Publisher) && b.IsDeleted == false);
        }
        public async Task<Book> GetByTitleAsync(string title)
        {
            _libraryContext.Books.AsNoTracking();

            var book = await Get(b => b.Title.Equals(title/*, StringComparison.OrdinalIgnoreCase*/) && b.IsDeleted == false);

            return book;
        }

        public async Task<Book> GetByIdAsync(Guid id)
        {
            var book = await Get(b => b.Id == id && b.IsDeleted == false);
            book.Author = await _libraryContext.Authors.FirstOrDefaultAsync(b => b.Id == book.AuthorId);

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
            bookToUpdate.PictureSource = book.PictureSource;
            bookToUpdate.Description = book.Description;
            //bookToUpdate.LastUpdatedAt = DateTime.UtcNow;

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

            bookToUpdate.Rating = (Rating)(((int)book.Rating) + ((int)bookToUpdate.Rating) / 2);

            await _libraryContext.SaveChangesAsync();

            return true;
        }
    }
}
