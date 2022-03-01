using System.Net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILoggingService _loggingService;

    public UsersController(IUserRepository userRepository, ILoggingService loggingService)
    {
        _userRepository = userRepository;
        _loggingService = loggingService;
    }

    [HttpGet]
    [SwaggerResponse(200, "Got all users", typeof(Response<IList<UserDto>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<UserDto>>>> GetUsers()
    {
        try
        {
            return Ok(await _userRepository.GetUsers());
        }
        catch (Exception ex)
        {
            return await _loggingService.AddDebugLog(Request, nameof(GetUsers), ex.Message, ex.StackTrace);
        }
    }

    [HttpGet("{id:int}")]
    [SwaggerResponse(200, "Got user", typeof(Response<User>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<User>>> GetUser(int id)
    {
        try
        {
            return Ok(await _userRepository.GetUser(id));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return await _loggingService.AddDebugLog(Request, nameof(GetUser), ex.Message, ex.StackTrace);
        }
    }

    [HttpGet("roles")]
    [SwaggerResponse(200, "Got all roles", typeof(Response<IList<Role>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<IList<Role>>>> GetRoles()
    {
        try
        {
            return Ok(await _userRepository.GetRoles());
        }
        catch (Exception ex)
        {
            return await _loggingService.AddDebugLog(Request, nameof(GetRoles), ex.Message, ex.StackTrace);
        }
    }

    [HttpPut]
    [SwaggerResponse(200, "Updated user", typeof(Response<User>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<User>>> UpdateUser([FromBody] User user)
    {
        try
        {
            return Ok(await _userRepository.UpdateUser(user, Request));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return await _loggingService.AddDebugLog(Request, nameof(UpdateUser), ex.Message, ex.StackTrace);
        }
    }

    [HttpPut("role")]
    [SwaggerResponse(200, "Added role", typeof(Response<User>))]
    [SwaggerResponse(404, "User or role not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<User>>> AddRole(int id, int roleId)
    {
        try
        {
            return Ok(await _userRepository.AddRole(id, roleId, Request));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return await _loggingService.AddDebugLog(Request, nameof(AddRole), ex.Message, ex.StackTrace);
        }
    }

    [HttpDelete("role")]
    [SwaggerResponse(200, "Removed role", typeof(Response<User>))]
    [SwaggerResponse(404, "User or role not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<User>>> RemoveRole(int id, int roleId)
    {
        try
        {
            return Ok(await _userRepository.RemoveRole(id, roleId, Request));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return await _loggingService.AddDebugLog(Request, nameof(RemoveRole), ex.Message, ex.StackTrace);
        }
    }

    [HttpDelete("{id:int}")]
    [SwaggerResponse(200, "Removed user", typeof(Response<User>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<User>>> DeleteUser(int id)
    {
        try
        {
            return Ok(await _userRepository.DeleteUser(id, Request));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new Response<string>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = ex.Message,
                Data = string.Empty
            });
        }
        catch (Exception ex)
        {
            return await _loggingService.AddDebugLog(Request, nameof(DeleteUser), ex.Message, ex.StackTrace);
        }
    }
}