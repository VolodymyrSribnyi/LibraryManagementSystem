using Application.DTOs.Users;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<GetUserDTO> CreateUserAsync(CreateUserDTO createUserDTO);
        Task<GetUserDTO> UpdateUserAsync(UpdateUserDTO updateUserDTO);
        Task<bool> Logout();
        Task<bool> DeleteUserAsync(Guid id);
        Task<GetUserDTO> GetUserByIdAsync(Guid id);
        Task<GetUserDTO> GetUserByEmailAsync(string email);
        Task<IEnumerable<GetUserDTO>> GetAllUsersAsync();
        Task<GetUserDTO> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO);
        Task<GetUserDTO> AuthenticateAsync(LoginUserDTO loginUserDTO);
        Task<IEnumerable<GetUserDTO>> GetUsersByRoleAsync(string roleName);
    }
}
