using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Repositories
{
    /// <summary>
    /// Defines a contract for managing and retrieving <see cref="Author"/> entities in a data store.
    /// </summary>
    /// <remarks>This interface provides methods for adding, updating, deleting, and retrieving <see
    /// cref="Author"/> entities. It supports both single-entity and collection-based operations, as well as filtering
    /// and eager loading of related data through navigation properties.</remarks>
    public interface IAuthorRepository
    {
        /// <summary>
        /// Asynchronously adds a new author to the data store.
        /// </summary>
        /// <param name="author">The <see cref="Author"/> object to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added <see cref="Author"/>
        /// object.</returns>
        Task<Author> AddAsync(Author author);
        /// <summary>
        /// Asynchronously updates the specified author's information in the data store.
        /// </summary>
        /// <param name="author">The <see cref="Author"/> object containing updated information. Cannot be null.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the updated <see cref="Author"/>
        /// object.</returns>
        Task<Author> UpdateAsync(Author author);
        /// <summary>
        /// Asynchronously deletes an entity identified by the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to be deleted.</param>
        /// <returns><see langword="true"/> if the entity was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<bool> DeleteAsync(Guid id);
        /// <summary>
        /// Retrieves a single <see cref="Author"/> entity that matches the specified filter criteria.
        /// </summary>
        /// <param name="filter">An expression used to filter the <see cref="Author"/> entities. This expression must evaluate to a boolean
        /// value.</param>
        /// <param name="includeProperties">A comma-separated list of related entities to include in the query results. This parameter is optional and
        /// can be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Author"/> entity
        /// that matches the filter criteria, or <c>null</c> if no match is found.</returns>
        Task<Author> Get(Expression<Func<Author, bool>> filter, string? includeProperties = null);
        /// <summary>
        /// Retrieves a collection of authors that match the specified filter criteria.
        /// </summary>
        /// <param name="filter">An optional expression to filter the authors. If null, all authors are retrieved.</param>
        /// <param name="includeProperties">A comma-separated list of related entities to include in the query results. If null, no related entities are
        /// included.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of
        /// authors that match the filter criteria.</returns>
        Task<IEnumerable<Author>> GetAll(Expression<Func<Author, bool>>? filter = null, string? includeProperties = null);
        Task<Author> GetByIdAsync(Guid id);
        /// <summary>
        /// Asynchronously retrieves an author by their full name.
        /// </summary>
        /// <param name="fullName">The full name of the author to retrieve. Cannot be null or empty.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the <see cref="Author"/> object if
        /// found; otherwise, <see langword="null"/>.</returns>
        Task<Author> GetByFullNameAsync(string fullName);
        /// <summary>
        /// Asynchronously retrieves all authors from the data source.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an  <see cref="IEnumerable{T}"/>
        /// of <see cref="Author"/> objects representing all authors.</returns>
        Task<IEnumerable<Author>> GetAllAsync();
    }
}
