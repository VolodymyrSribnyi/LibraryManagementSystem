using Application.DTOs.Authors;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<GetAuthorDTO> AddAsync(CreateAuthorDTO createAuthorDTO);
        Task<GetAuthorDTO> UpdateAsync(UpdateAuthorDTO updateAuthorDTO);
        Task<bool> DeleteAsync(Guid id);
        Task<GetAuthorDTO> GetByIdAsync(Guid id);
        Task<GetAuthorDTO> GetByFullNameAsync(string fullName);
        Task<IEnumerable<GetAuthorDTO>> GetAllAsync();
    }
}
