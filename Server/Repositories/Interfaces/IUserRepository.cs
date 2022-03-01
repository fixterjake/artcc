using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IUserRepository
{
    Task<Response<User>> CreateUser(User user, HttpRequest request);
    Task<Response<IList<UserDto>>> GetUsers();
    Task<Response<User>> GetUser(int id);
    Task<Response<IList<Role>>> GetRoles();
    Task<Response<User>> UpdateUser(User user, HttpRequest request);
    Task<Response<User>> AddRole(int id, int roleId, HttpRequest request);
    Task<Response<User>> RemoveRole(int id, int roleId, HttpRequest request);
    Task<Response<User>> DeleteUser(int id, HttpRequest request);
}