using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IUserRepository
{
    /// <summary>
    /// Get users
    /// </summary>
    /// <returns>All users</returns>
    Task<Response<IList<UserDto>>> GetUsers();

    /// <summary>
    /// Get user by id
    /// </summary>
    /// <param name="userId">User id</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>User if found</returns>
    Task<Response<User>> GetUser(int userId);

    /// <summary>
    /// Get roles
    /// </summary>
    /// <returns>Roles</returns>
    Task<Response<IList<Role>>> GetRoles();

    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="user">Updated user</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>Updated user</returns>
    Task<Response<User>> UpdateUser(User user, HttpRequest request);

    /// <summary>
    /// Add role to user
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="roleId">Role id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <exception cref="Shared.RoleNotFoundException">Role not found</exception>
    /// <returns>Updated user</returns>
    Task<Response<User>> AddRole(int userId, int roleId, HttpRequest request);

    /// <summary>
    /// Remove role from user
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="roleId">Role id</param>
    /// <param name="request">Raw http request</param>
    /// <returns>Updated user</returns>
    Task<Response<User>> RemoveRole(int userId, int roleId, HttpRequest request);

    /// <summary>
    /// Soft delete user
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.UserNotFoundException">User not found</exception>
    /// <returns>Deleted user</returns>
    Task<Response<User>> DeleteUser(int userId, HttpRequest request);
}