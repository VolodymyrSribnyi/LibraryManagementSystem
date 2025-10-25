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
        /// <summary>
        /// Retrieves a single <see cref="Book"/> entity that matches the specified filter criteria.
        /// </summary>
        /// <param name="filter">An expression used to filter the <see cref="Book"/> entities. This expression must evaluate to a boolean
        /// value.</param>
        /// <param name="includeProperties">A comma-separated list of related entities to include in the query results. This parameter is optional and
        /// can be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Book"/> entity
        /// that matches the filter criteria, or <see langword="null"/> if no match is found.</returns>
        Task<Book> Get(Expression<Func<Book, bool>> filter, string? includeProperties = null);
        /// <summary>
        /// Retrieves all books that match the specified filter criteria.
        /// </summary>
        /// <param name="filter">An optional expression to filter the books. If null, all books are retrieved.</param>
        /// <param name="includeProperties">A comma-separated list of related entities to include in the query results. If null, no related entities are
        /// included.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of
        /// books that match the filter criteria.</returns>
        Task<IEnumerable<Book>> GetAll(Expression<Func<Book, bool>>? filter = null, string? includeProperties = null);
        /// <summary>
        /// Asynchronously adds a new book to the collection.
        /// </summary>
        /// <param name="book">The book to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added book with updated
        /// information, such as an assigned ID.</returns>
        Task<Book> AddAsync(Book book);
        /// <summary>
        /// Asynchronously updates the specified book in the data store.
        /// </summary>
        /// <param name="book">The book object containing updated information. The book's identifier must match an existing entry in the
        /// data store.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated book object.</returns>
        Task<Book> UpdateAsync(Book book);
        /// <summary>
        /// Updates the availability status of the specified book asynchronously.
        /// </summary>
        /// <param name="book">The book whose availability status is to be updated. Cannot be null.</param>
        /// <returns><see langword="true"/> if the availability status was successfully updated; otherwise, <see
        /// langword="false"/>.</returns>
        Task<bool> UpdateAvailabilityAsync(Book book);
        /// <summary>
        /// Updates the rating of the specified book asynchronously.
        /// </summary>
        /// <param name="book">The book whose rating is to be updated. Cannot be null.</param>
        /// <returns><see langword="true"/> if the rating was successfully updated; otherwise, <see langword="false"/>.</returns>
        Task<bool> UpdateRatingAsync(Book book);
        /// <summary>
        /// Asynchronously deletes an entity identified by the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to be deleted.</param>
        /// <returns><see langword="true"/> if the entity was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<bool> DeleteAsync(Guid id);
        /// <summary>
        /// Asynchronously retrieves a collection of books that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="filter">An expression that defines the conditions each book must satisfy to be included in the result set.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of
        /// books that match the filter criteria.</returns>
        Task<IEnumerable<Book>> GetFilteredAsync(Expression<Func<Book,bool>> filter);
        /// <summary>
        /// Asynchronously retrieves a book by its title.
        /// </summary>
        /// <param name="title">The title of the book to retrieve. Cannot be null or empty.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the <see cref="Book"/> object with
        /// the specified title, or <see langword="null"/> if no such book is found.</returns>
        Task<Book> GetByTitleAsync(string title);
        /// <summary>
        /// Asynchronously retrieves a book by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the book to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Book"/> object if
        /// found; otherwise, <see langword="null"/>.</returns>
        Task<Book> GetByIdAsync(Guid id);
        /// <summary>
        /// Asynchronously retrieves all books from the data source.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an  <IEnumerable{T}/> of <Book>
        /// objects representing all books.</returns>
        Task<IEnumerable<Book>> GetAllAsync();
        /// <summary>
        /// Asynchronously retrieves all books written by the specified author.
        /// </summary>
        /// <param name="author">The author whose books are to be retrieved. Cannot be null.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an enumerable collection of books
        /// written by the specified author. The collection will be empty if the author has no books.</returns>
        Task<IEnumerable<Book>> GetAllByAuthorAsync(Author author);
        /// <summary>
        /// Asynchronously retrieves all books published by the specified publishers.
        /// </summary>
        /// <param name="publishers">A collection of publisher names to filter the books by. Cannot be null or empty.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an enumerable collection of books
        /// published by the specified publishers.</returns>
        Task<IEnumerable<Book>> GetAllByPublisherAsync(IEnumerable<string> publishers);
        /// <summary>
        /// Asynchronously retrieves all books that match the specified genres.
        /// </summary>
        /// <param name="genres">A collection of genres to filter the books by. Cannot be null or empty.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an enumerable of books that belong
        /// to the specified genres. The enumerable will be empty if no books match the genres.</returns>
        Task<IEnumerable<Book>> GetAllByGenresAsync(IEnumerable<Genre> genres);
    }
}
