using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abstractions.Repositories
{
    //    + Add(IBook book) : IBook
    //+ Update(Guid id, IBook book) : IBook
    //+ Delete(Guid id) : bool
    //+ GetByTitle(string title) : IBook
    //+ GetById(int or GUID) : IBook
    //+ GetAll() : List<IBook>
    //+ GetAllByAuthor(IAuthor author) : List<IBook>
    //+ GetAllByPublisher(List<string> Publisher) : List<IBook>
    //+ GetAllByGenres(enum genres) : List<IBook>
    public interface IBookRepository
    {
        Task<Book> AddAsync(Book book);
        Task<Book> UpdateAsync(Book book);
        Task<bool> UpdateAvailabilityAsync(Book book);
        Task<bool> UpdateRatingAsync(Book book);
        Task<bool> DeleteAsync(Guid id);
        Task<Book> GetByTitleAsync(string title);
        Task<Book> GetByIdAsync(Guid id);
        Task<IEnumerable<Book>> GetAllAsync();
        Task<IEnumerable<Book>> GetAllByAuthorAsync(Author author);
        Task<IEnumerable<Book>> GetAllByPublisherAsync(IEnumerable<string> publishers);
        Task<IEnumerable<Book>> GetAllByGenresAsync(IEnumerable<Genre> genres);
    }
}
