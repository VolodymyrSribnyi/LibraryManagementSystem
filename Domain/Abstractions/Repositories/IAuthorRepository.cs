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
        Task<Author> AddAsync(Author author);
        Task<Author> UpdateAsync(Author author);
        Task<bool> DeleteAsync(Guid id);
        Task<Author> Get(Expression<Func<Author, bool>> filter, string? includeProperties = null);
        Task<IEnumerable<Author>> GetAll(Expression<Func<Author, bool>>? filter = null, string? includeProperties = null);
        Task<Author> GetByIdAsync(Guid id);
        Task<Author> GetByFullNameAsync(string fullName);
        Task<IEnumerable<Author>> GetAllAsync();
    }
}
