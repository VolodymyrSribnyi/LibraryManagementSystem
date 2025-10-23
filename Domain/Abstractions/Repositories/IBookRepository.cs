using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Repositories
{
    /// <summary>
    /// Defines a contract for managing and querying book entities in a data store.
    /// </summary>
    /// <remarks>This interface provides methods for adding, updating, deleting, and retrieving books,  as
    /// well as querying books based on various criteria such as title, author, publisher, or genres.  It supports
    /// asynchronous operations to ensure scalability and responsiveness in data access.</remarks>
    public interface IBookRepository
    {
        Task<Book> Get(Expression<Func<Book, bool>> filter, string? includeProperties = null);
        Task<IEnumerable<Book>> GetAll(Expression<Func<Book, bool>>? filter = null, string? includeProperties = null);
        Task<Book> AddAsync(Book book);
        Task<Book> UpdateAsync(Book book);
        Task<bool> UpdateAvailabilityAsync(Book book);
        Task<bool> UpdateRatingAsync(Book book);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<Book>> GetFilteredAsync(Expression<Func<Book,bool>> filter);
        Task<Book> GetByTitleAsync(string title);
        Task<Book> GetByIdAsync(Guid id);
        Task<IEnumerable<Book>> GetAllAsync();
        Task<IEnumerable<Book>> GetAllByAuthorAsync(Author author);
        Task<IEnumerable<Book>> GetAllByPublisherAsync(IEnumerable<string> publishers);
        Task<IEnumerable<Book>> GetAllByGenresAsync(IEnumerable<Genre> genres);
    }
}
