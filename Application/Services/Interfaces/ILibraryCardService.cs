using Application.DTOs.LibraryCards;
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
        Task<GetLibraryCardDTO> CreateAsync(Guid userId);
        Task<GetLibraryCardDTO> UpdateAsync(UpdateLibraryCardDTO updateLibraryCardDTO);
        Task<bool> DeleteAsync(Guid userId);
        Task<bool> IsActiveAsync(Guid userId);
        Task<DateTime?> GetExpirationDateAsync(Guid userId);
        Task<IEnumerable<GetLibraryCardDTO>> GetAllAsync();
    }
}
