﻿using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    private readonly ILoggingService _loggingService;
    private readonly IMapper _mapper;

    public UserRepository(DatabaseContext context, ILoggingService loggingService, IMapper mapper)
    {
        _context = context;
        _loggingService = loggingService;
        _mapper = mapper;
    }

    #region Create

    public async Task<Response<User>> CreateUser(User user, HttpRequest request)
    {
        var result = await _context.AddAsync(user);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created user '{result.Entity.Id}'", string.Empty, newData);

        return new Response<User>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created user '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Read

    public async Task<Response<IList<UserDto>>> GetUsers()
    {
        var result = await _context.Users.ToListAsync();
        var users = _mapper.Map<IList<UserDto>>(result);
        return new Response<IList<UserDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {users.Count} users",
            Data = users
        };
    }

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