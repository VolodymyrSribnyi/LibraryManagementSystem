using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Repositories
{
    //    + Add(IAuthor Author) : IAuthor
    //+ Update(Guid id, IAuthor Author) : IAuthor
    //+ Delete(Guid id) : bool
    //+ GetById(Guid id) : IAuthor
    //+ GetByFullName(string FullName) : IAuthor
    //+GetAll() :List<IAuthor>
    public interface IAuthorRepository
    {
        Task<Author> AddAsync(Author author);
        Task<Author> UpdateAsync(Author author);
        Task<bool> DeleteAsync(Guid id);
        Task<Author> GetByIdAsync(Guid id);
        Task<Author> GetByFullNameAsync(string fullName);
        Task<IEnumerable<Author>> GetAllAsync();
    }
}
