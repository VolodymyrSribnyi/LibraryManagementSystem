using Application.DTOs.Users;
using Application.ErrorHandling;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    /// <summary>
    /// Defines a contract for managing user-related operations, including authentication, creating, updating,
    /// retrieving, and deleting users.
    /// </summary>
    /// <remarks>This service provides methods for performing CRUD operations on users, as well as handling
    /// authentication, password management, and role-based user retrieval. Implementations of this interface are
    /// expected to handle the underlying data access and business logic.</remarks>
    public interface IUserService
    {
        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="createUserDTO">The data transfer object containing the information for the new user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetUserDTO"/> representing the created user.</returns>
        Task<Result<GetUserDTO>> CreateUserAsync(CreateUserDTO createUserDTO);

        /// <summary>
        /// Updates an existing user's information.
        /// </summary>
        /// <param name="updateUserDTO">The data transfer object containing the updated user information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetUserDTO"/> representing the updated user.</returns>
        Task<Result<GetUserDTO>> UpdateUserAsync(UpdateUserDTO updateUserDTO);

        /// <summary>
        /// Logs out the current user from the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the logout was successful; otherwise, <see langword="false"/>.</returns>
        Task<Result> Logout();

        /// <summary>
        /// Deletes a user from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the user was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<Result> DeleteUserAsync(Guid id);

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetUserDTO"/> representing the user if found; otherwise, <see langword="null"/>.</returns>
        Task<Result<GetUserDTO>> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetUserDTO"/> representing the user if found; otherwise, <see langword="null"/>.</returns>
        Task<Result<GetUserDTO>> GetUserByEmailAsync(string email);

        /// <summary>
        /// Retrieves all users in the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="GetUserDTO"/> objects representing all users.</returns>
        Task<IEnumerable<GetUserDTO>> GetAllUsersAsync();

        /// <summary>
        /// Changes the password for a user.
        /// </summary>
        /// <param name="changePasswordDTO">The data transfer object containing the password change information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetUserDTO"/> representing the user with the updated password.</returns>
        Task<Result<GetUserDTO>> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO);

        /// <summary>
        /// Authenticates a user with their login credentials.
        /// </summary>
        /// <param name="loginUserDTO">The data transfer object containing the user's login credentials.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="GetUserDTO"/> representing the authenticated user if successful; otherwise, <see langword="null"/>.</returns>
        Task<Result<GetUserDTO>> AuthenticateAsync(LoginUserDTO loginUserDTO);

        /// <summary>
        /// Retrieves all users assigned to a specific role.
        /// </summary>
        /// <param name="roleName">The name of the role to filter users by.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="GetUserDTO"/> objects representing users with the specified role.</returns>
        Task<Result<IEnumerable<GetUserDTO>>> GetUsersByRoleAsync(string roleName);
    }
}
