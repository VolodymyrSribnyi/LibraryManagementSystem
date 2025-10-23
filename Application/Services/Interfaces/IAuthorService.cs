using Application.DTOs.Authors;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    /// <summary>
    /// Defines a contract for managing author-related operations, including creating, updating, retrieving, and
    /// deleting authors.
    /// </summary>
    /// <remarks>This service provides methods for performing CRUD operations on authors, as well as mapping
    /// between data transfer objects. Implementations of this interface are expected to handle the underlying data
    /// access and business logic.</remarks>
    public interface IAuthorService
    {
        Task<GetAuthorDTO> AddAsync(CreateAuthorDTO createAuthorDTO);
        Task<GetAuthorDTO> UpdateAsync(UpdateAuthorDTO updateAuthorDTO);
        Task<bool> DeleteAsync(Guid id);
        Task<GetAuthorDTO> GetByIdAsync(Guid id);
        Task<GetAuthorDTO> GetByFullNameAsync(string fullName);
        Task<IEnumerable<GetAuthorDTO>> GetAllAsync();
        UpdateAuthorDTO MapToUpdateAuthorDTO(GetAuthorDTO getAuthorDTO);
    }
}
