using Application.DTOs.Users;
using Application.ErrorHandling;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    /// <summary>
    /// Provides functionality for managing users, including user creation, authentication,
    /// profile management, and role-based operations.
    /// </summary>
    /// <remarks>
    /// This service integrates with ASP.NET Core Identity for user management and authentication,
    /// ensuring that business rules are enforced when handling user operations.
    /// </remarks>
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

        public async Task<Result<GetUserDTO>> CreateUserAsync(CreateUserDTO createUserDTO)
        {
            if (createUserDTO == null)
            {
                _logger.LogWarning("CreateUserAsync called with null CreateUserDTO.");
                return Result<GetUserDTO>.Failure(Errors.NullData);
            }

            var existingUser = await _userManager.FindByEmailAsync(createUserDTO.Email);

            if (existingUser != null)
            {
                _logger.LogInformation($"User with email {createUserDTO.Email} already exists.");
                return Result<GetUserDTO>.Failure(Errors.UserExists);
            }

            var user = _mapper.Map<ApplicationUser>(createUserDTO);
            user.Id = Guid.NewGuid();
            user.CreatedAt = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(user, createUserDTO.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError($"Failed to create user: {errors}");
                return Result<GetUserDTO>.Failure(new Error("UserCreationFailed", $"Failed to create user: {errors}"));
            }

            var usersCount = _userManager.Users.Count();
            string role = usersCount == 1 ? "Admin" : "User";
            await _userManager.AddToRoleAsync(user, role);

            _logger.LogInformation($"Successfully created user with ID: {user.Id} and assigned role: {role}");
            return Result<GetUserDTO>.Success(_mapper.Map<GetUserDTO>(user));
        }
        public async Task<Result<GetUserDTO>> UpdateUserAsync(UpdateUserDTO updateUserDTO)
        {
            if (updateUserDTO == null)
            {
                _logger.LogWarning("UpdateUserAsync called with null UpdateUserDTO.");
                return Result<GetUserDTO>.Failure(Errors.NullData);
            }

            var user = await _userManager.FindByIdAsync(updateUserDTO.Id.ToString());

            if (user == null)
            {
                _logger.LogInformation($"User with ID {updateUserDTO.Id} not found.");
                return Result<GetUserDTO>.Failure(Errors.UserNotFound);
            }

            _mapper.Map(updateUserDTO, user);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError($"Failed to update user {updateUserDTO.Id}: {errors}");
                return Result<GetUserDTO>.Failure(new Error("UserUpdateFailed", $"Failed to update user: {errors}"));
            }

            _logger.LogInformation($"Successfully updated user with ID: {user.Id}");
            return Result<GetUserDTO>.Success(_mapper.Map<GetUserDTO>(user));
        }
        public async Task<Result> DeleteUserAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("DeleteUserAsync called with empty user ID.");
                return Result.Failure(Errors.NullData);
            }

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                _logger.LogInformation($"User with ID {id} not found.");
                return Result.Failure(Errors.UserNotFound);
            }

            if (user.ReservedBooks != null && user.ReservedBooks.Any(r => r.EndsAt > DateTime.UtcNow))
            {
                _logger.LogWarning($"Cannot delete user {id} with active reservations.");
                return Result.Failure(Errors.UserHasActiveReservations);
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError($"Failed to delete user {id}: {errors}");
                return Result.Failure(new Error("UserDeletionFailed", $"Failed to delete user: {errors}"));
            }

            _logger.LogInformation($"Successfully deleted user with ID: {id}");
            return Result.Success();
        }
        public async Task<IEnumerable<GetUserDTO>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            if (users == null || !users.Any())
            {
                _logger.LogInformation("No users found in the system.");
                return Enumerable.Empty<GetUserDTO>();
            }

            _logger.LogInformation($"Retrieved {users.Count} users.");
            return _mapper.Map<IEnumerable<GetUserDTO>>(users);
        }

        public async Task<Result<GetUserDTO>> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("GetUserByEmailAsync called with null or empty email.");
                return Result<GetUserDTO>.Failure(Errors.NullData);
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                _logger.LogInformation($"User with email {email} not found.");
                return Result<GetUserDTO>.Failure(Errors.UserNotFound);
            }

            return Result<GetUserDTO>.Success(_mapper.Map<GetUserDTO>(user));
        }

        public async Task<Result<GetUserDTO>> GetUserByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("GetUserByIdAsync called with empty user ID.");
                return Result<GetUserDTO>.Failure(Errors.NullData);
            }

            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                _logger.LogInformation($"User with ID {id} not found.");
                return Result<GetUserDTO>.Failure(Errors.UserNotFound);
            }

            return Result<GetUserDTO>.Success(_mapper.Map<GetUserDTO>(user));
        }

        public async Task<Result<IEnumerable<GetUserDTO>>> GetUsersByRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                _logger.LogWarning("GetUsersByRoleAsync called with null or empty role name.");
                return Result<IEnumerable<GetUserDTO>>.Failure(Errors.NullData);
            }

            var usersWithRole = await _userManager.GetUsersInRoleAsync(roleName);

            if (usersWithRole == null || !usersWithRole.Any())
            {
                _logger.LogInformation($"No users found for role {roleName}.");
                return Result<IEnumerable<GetUserDTO>>.Failure(Errors.UsersNotFoundForRole);
            }

            _logger.LogInformation($"Retrieved {usersWithRole.Count} users for role {roleName}.");
            return Result<IEnumerable<GetUserDTO>>.Success(_mapper.Map<IEnumerable<GetUserDTO>>(usersWithRole));
        }


        public async Task<Result<GetUserDTO>> AuthenticateAsync(LoginUserDTO loginUserDTO)
        {
            if (loginUserDTO == null)
            {
                _logger.LogWarning("AuthenticateAsync called with null LoginUserDTO.");
                return Result<GetUserDTO>.Failure(Errors.NullData);
            }

            var applicationUser = await _userManager.FindByNameAsync(loginUserDTO.UserName);

            if (applicationUser == null)
            {
                _logger.LogInformation($"User with username {loginUserDTO.UserName} not found.");
                return Result<GetUserDTO>.Failure(Errors.UserNotFound);
            }

            var signInResult = await _signInManager.PasswordSignInAsync(
                loginUserDTO.UserName,
                loginUserDTO.Password,
                loginUserDTO.RememberMe,
                lockoutOnFailure: false
            );

            if (!signInResult.Succeeded)
            {
                _logger.LogWarning($"Invalid login attempt for user {loginUserDTO.UserName}.");
                return Result<GetUserDTO>.Failure(Errors.InvalidLoginAttempt);
            }

            _logger.LogInformation($"User {applicationUser.UserName} logged in successfully.");
            return Result<GetUserDTO>.Success(_mapper.Map<GetUserDTO>(applicationUser));
        }
        public async Task<Result> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out successfully.");
            return Result.Success();
        }
        public async Task<Result<GetUserDTO>> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            if (changePasswordDTO == null)
            {
                _logger.LogWarning("ChangePasswordAsync called with null ChangePasswordDTO.");
                return Result<GetUserDTO>.Failure(Errors.NullData);
            }

            var user = await _userManager.FindByIdAsync(changePasswordDTO.UserId.ToString());

            if (user == null)
            {
                _logger.LogInformation($"User with ID {changePasswordDTO.UserId} not found.");
                return Result<GetUserDTO>.Failure(Errors.UserNotFound);
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning($"Failed to change password for user {changePasswordDTO.UserId}: {errors}");
                return Result<GetUserDTO>.Failure(new Error("PasswordChangeFailed", $"Failed to change password: {errors}"));
            }

            _logger.LogInformation($"Successfully changed password for user ID: {user.Id}");
            return Result<GetUserDTO>.Success(_mapper.Map<GetUserDTO>(user));
        }
    }
}
