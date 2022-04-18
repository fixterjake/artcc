using System.Net;
using Amazon.Runtime.Internal.Util;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILoggingService _loggingService;
    private readonly IMapper _mapper;

    public UserRepository(DatabaseContext context, IDistributedCache cache, ILoggingService loggingService, IMapper mapper)
    {
        _context = context;
        _cache = cache;
        _loggingService = loggingService;
        _mapper = mapper;
    }

    #region Read

    /// <inheritdoc />
    public async Task<Response<IList<UserDto>>> GetUsers()
    {
        var cachedUsers = await _cache.GetStringAsync("_users");
        if (!string.IsNullOrEmpty(cachedUsers))
        {
            var users = JsonConvert.DeserializeObject<List<UserDto>>(cachedUsers);
            if (users != null)
                return new Response<IList<UserDto>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $"Got {users.Count} users",
                    Data = users
                };
        }

        var resultRaw = await _context.Users.ToListAsync();
        var result = _mapper.Map<IList<UserDto>>(resultRaw);
        var expiryOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };
        await _cache.SetStringAsync("_users", JsonConvert.SerializeObject(result), expiryOptions);

        return new Response<IList<UserDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {result.Count} users",
            Data = result
        };
    }

    /// <inheritdoc />
    public async Task<Response<User>> GetUser(int userId)
    {
        var user = await _context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == userId) ??
            throw new UserNotFoundException($"User '{userId}' not found");
        return new Response<User>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got user '{userId}'",
            Data = user
        };
    }

    /// <inheritdoc />
    public async Task<Response<IList<Role>>> GetRoles()
    {
        var roles = await _context.Roles.ToListAsync();
        return new Response<IList<Role>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {roles.Count} roles",
            Data = roles
        };
    }

    #endregion

    #region Update

    /// <inheritdoc />
    public async Task<Response<User>> UpdateUser(User user, HttpRequest request)
    {
        var dbUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id) ??
            throw new UserNotFoundException($"User '{user.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbUser);
        var result = _context.Users.Update(user);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated user '{user.Id}'", oldData, newData);

        return new Response<User>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Updated user '{user.Id}'",
            Data = result.Entity
        };
    }

    /// <inheritdoc />
    public async Task<Response<User>> AddRole(int userId, int roleId, HttpRequest request)
    {
        var user = await _context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == userId) ??
            throw new UserNotFoundException($"User '{userId}' not found");
        var role = await _context.Roles.FindAsync(roleId) ??
            throw new RoleNotFoundException($"Role '{roleId}' not found");

        var oldData = JsonConvert.SerializeObject(user.Roles);
        user.Roles?.Add(role);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(user.Roles);

        await _loggingService.AddWebsiteLog(request, $"Added role '{roleId}' to user '{userId}'", oldData, newData);

        return new Response<User>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Added role '{roleId}' to user '{userId}'",
            Data = user
        };
    }

    /// <inheritdoc />
    public async Task<Response<User>> RemoveRole(int userId, int roleId, HttpRequest request)
    {
        var user = await _context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == userId) ??
            throw new UserNotFoundException($"User '{userId}' not found");
        var role = await _context.Roles.FindAsync(roleId) ??
            throw new RoleNotFoundException($"Role '{roleId}' not found");

        var oldData = JsonConvert.SerializeObject(user.Roles);
        user.Roles?.Remove(role);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(user.Roles);

        await _loggingService.AddWebsiteLog(request, $"Added role '{roleId}' to user '{userId}'", oldData, newData);

        return new Response<User>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Added role '{roleId}' to user '{userId}'",
            Data = user
        };
    }

    #endregion

    #region Delete

    /// <inheritdoc />
    public async Task<Response<User>> DeleteUser(int userId, HttpRequest request)
    {
        var user = await _context.Users.FindAsync(userId) ??
            throw new UserNotFoundException($"User '{userId}' not found");

        var oldData = JsonConvert.SerializeObject(user);
        user.Status = UserStatus.Removed;
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(user);

        await _loggingService.AddWebsiteLog(request, $"Removed user '{userId}'", oldData, newData);

        return new Response<User>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Removed user '{userId}'",
            Data = user
        };
    }

    #endregion
}