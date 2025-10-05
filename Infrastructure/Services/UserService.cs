using Application.DTOs.Users;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,IMapper mapper,ILogger<UserService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _logger = logger;
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
                throw new UserExistsException($"User with email {createUserDTO.Email} already exists.");
            }

            var user = _mapper.Map<ApplicationUser>(createUserDTO);
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(user, createUserDTO.Password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            _logger.LogInformation($"User created with ID: {user.Id}");
            return _mapper.Map<GetUserDTO>(user);
        }
        public async Task<GetUserDTO> UpdateUserAsync(UpdateUserDTO updateUserDTO)
        {
            if(updateUserDTO == null)
                throw new ArgumentNullException(nameof(updateUserDTO));

            var user = await _userManager.FindByIdAsync(updateUserDTO.Id.ToString());

            if (user == null)
            {
                throw new UserNotFoundException($"User with ID {updateUserDTO.Id} not found.");
            }

            _mapper.Map(updateUserDTO, user);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            _logger.LogInformation($"User with ID: {user.Id} updated successfully.");
            return _mapper.Map<GetUserDTO>(user);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id), "User ID cannot be empty");

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                throw new UserNotFoundException($"User with ID {id} not found.");

            if (user.ReservedBooks.Any(r => r.EndsAt > DateTime.Now))
                throw new InvalidOperationException("User cannot be deleted while having active reservations.");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            _logger.LogInformation($"User with ID: {id} deleted successfully.");
            return result.Succeeded;
        }

        public async Task<IEnumerable<GetUserDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            if (users == null || !users.Any())
            {
                throw new UserNotFoundException("No users found.");
            }

            return _mapper.Map<IEnumerable<GetUserDTO>>(users);
        }

        public async Task<GetUserDTO> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email), "Email cannot be null or empty");

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new UserNotFoundException($"User with email {email} not found.");

            return _mapper.Map<GetUserDTO>(user);
        }

        public async Task<GetUserDTO> GetUserByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentNullException(nameof(id), "User ID cannot be empty");

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                throw new UserNotFoundException($"User with ID {id} not found.");
            }

            return _mapper.Map<GetUserDTO>(user);
        }

        public async Task<IEnumerable<GetUserDTO>> GetUsersByRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName), "Role name cannot be null or empty");

            var usersWithRole = await _userManager.GetUsersInRoleAsync(roleName);

            if (usersWithRole == null || !usersWithRole.Any())
            {
                throw new UserNotFoundException($"No users found for role {roleName}.");
            }

            return _mapper.Map<IEnumerable<GetUserDTO>>(usersWithRole);
        }

        
        public async Task<GetUserDTO> AuthenticateAsync(LoginUserDTO loginUserDTO)
        {
            if (loginUserDTO == null)
                throw new ArgumentNullException(nameof(loginUserDTO), "Login data cannot be null");

            var user = await _signInManager.PasswordSignInAsync(
                loginUserDTO.UserName,
                loginUserDTO.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!user.Succeeded)
            {
                throw new InvalidOperationException("Invalid login attempt.");
            }

            var applicationUser = await _userManager.FindByNameAsync(loginUserDTO.UserName);

            if (applicationUser == null)
            {
                throw new UserNotFoundException("User not found.");
            }

            _logger.LogInformation($"User {applicationUser.UserName} logged in successfully.");
            return _mapper.Map<GetUserDTO>(applicationUser);
        }
        public async Task<bool> Logout()
        {
            await _signInManager.SignOutAsync();

            _logger.LogInformation("User logged out successfully.");
            return true;
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

            _logger.LogInformation($"Password changed successfully for user ID: {user.Id}");
            return _mapper.Map<GetUserDTO>(user);
        }
    }
}
