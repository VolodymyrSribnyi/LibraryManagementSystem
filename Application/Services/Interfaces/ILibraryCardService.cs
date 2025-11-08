using Application.DTOs.LibraryCards;
using Application.ErrorHandling;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ILibraryCardService
    {
        Task<Result<GetLibraryCardDTO>> CreateAsync(Guid userId);
        Task<Result<GetLibraryCardDTO>> UpdateAsync(UpdateLibraryCardDTO updateLibraryCardDTO);
        Task<Result> DeleteAsync(Guid userId);
        Task<Result> IsActiveAsync(Guid userId);
        Task<Result<DateTime?>> GetExpirationDateAsync(Guid userId);
        Task<Result<IEnumerable<GetLibraryCardDTO>>> GetAllAsync();
        Task<Result<bool>> IsExistsAsync(Guid userId);
    }
}
