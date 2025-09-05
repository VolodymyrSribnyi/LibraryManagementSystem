using Application.DTOs.Users;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public async Task<GetUserDTO> CreateUserAsync(CreateUserDTO createUserDTO)
        {
            if (createUserDTO == null)
            {
                throw new ArgumentNullException(nameof(createUserDTO), "CreateUserDTO cannot be null");
            }

            var existingUser = await _userManager.FindByEmailAsync(createUserDTO.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with email {createUserDTO.Email} already exists.");
            }

            var user = _mapper.Map<ApplicationUser>(createUserDTO);
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(user, createUserDTO.Password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return _mapper.Map<GetUserDTO>(user);
        }
        public async Task<GetUserDTO> UpdateUserAsync(UpdateUserDTO updateUserDTO)
        {
            var user = await _userManager.FindByIdAsync(updateUserDTO.Id.ToString());

            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {updateUserDTO.Id} not found.");
            }

            _mapper.Map(updateUserDTO, user);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return _mapper.Map<GetUserDTO>(user);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {id} not found.");
            }

            if (user.ReservedBooks.Any(r => r.EndsAt > DateTime.Now))
            {
                throw new InvalidOperationException("User cannot be deleted while having active reservations.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return result.Succeeded;
        }

        public async Task<IEnumerable<GetUserDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            if (users == null || !users.Any())
            {
                throw new InvalidOperationException("No users found.");
            }

            return _mapper.Map<IEnumerable<GetUserDTO>>(users);
        }

        public async Task<GetUserDTO> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new InvalidOperationException($"User with email {email} not found.");
            }

            return _mapper.Map<GetUserDTO>(user);
        }

        public async Task<GetUserDTO> GetUserByIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {id} not found.");
            }

            return _mapper.Map<GetUserDTO>(user);
        }

        public async Task<IEnumerable<GetUserDTO>> GetUsersByRoleAsync(string roleName)
        {
            var usersWithRole = await _userManager.GetUsersInRoleAsync(roleName);

            if (usersWithRole == null || !usersWithRole.Any())
            {
                throw new InvalidOperationException($"No users found for role {roleName}.");
            }

            return _mapper.Map<IEnumerable<GetUserDTO>>(usersWithRole);
        }

        
        public async Task<GetUserDTO> AuthenticateAsync(LoginUserDTO loginUserDTO)
        {
            var user = await _signInManager.PasswordSignInAsync(
                loginUserDTO.Email,
                loginUserDTO.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!user.Succeeded)
            {
                throw new InvalidOperationException("Invalid login attempt.");
            }

            var applicationUser = await _userManager.FindByEmailAsync(loginUserDTO.Email);

            if (applicationUser == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            return _mapper.Map<GetUserDTO>(applicationUser);
        }
        public async Task<GetUserDTO> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            var user = await _userManager.FindByIdAsync(changePasswordDTO.UserId.ToString());

            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {changePasswordDTO.UserId} not found.");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to change password: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return _mapper.Map<GetUserDTO>(user);
        }
    }
}
