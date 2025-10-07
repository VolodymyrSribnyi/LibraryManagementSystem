using Application.DTOs.Authors;
using Application.DTOs.Books;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IBookService
    {
        Task<GetBookDTO> AddAsync(CreateBookDTO createBookDTO);
        Task<GetBookDTO> UpdateAsync(UpdateBookDTO updateBookDTO);
        Task<bool> UpdateAvailabilityAsync(UpdateBookStatusDTO updateBookStatusDTO);
        Task<bool> UpdateRatingAsync(UpdateBookRatingDTO updateBookRatingDTO);
        Task<bool> DeleteAsync(Guid id);
        Task<GetBookDTO> GetByTitleAsync(string title);
        Task<GetBookDTO> GetByIdAsync(Guid id);
        Task<IEnumerable<GetBookDTO>> GetAllAsync();
        Task<IEnumerable<GetBookDTO>> GetAllByAuthorAsync(GetAuthorDTO getAuthorDTO);
        Task<IEnumerable<GetBookDTO>> GetAllByPublisherAsync(IEnumerable<string> publishers);
        Task<IEnumerable<GetBookDTO>> GetAllByGenresAsync(IEnumerable<Genre> genres);
        Task<byte[]> GetBookPictureAsync(Guid bookId);
    }
}
