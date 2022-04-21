using Microsoft.AspNetCore.Mvc;
using Sentry;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;
using User = ZDC.Shared.Models.User;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IHub _sentryHub;

    public UsersController(IUserRepository userRepository, IHub sentryHub)
    {
        _userRepository = userRepository;
        _sentryHub = sentryHub;
    }

    #region Read

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
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("{userId:int}")]
    [SwaggerResponse(200, "Got user", typeof(Response<User>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<User>>> GetUser(int userId)
    {
        try
        {
            return Ok(await _userRepository.GetUser(userId));
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
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpGet("roles")]
    // todo auth
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
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Update

    [HttpPut]
    // todo auth
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
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpPut("role/{userId:int}/{roleId:int}")]
    // todo auth
    [SwaggerResponse(200, "Added role", typeof(Response<User>))]
    [SwaggerResponse(404, "User or role not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<User>>> AddRole(int userId, int roleId)
    {
        try
        {
            return Ok(await _userRepository.AddRole(userId, roleId, Request));
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
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    [HttpDelete("role{userId:int}/{roleId:int}")]
    // todo auth
    [SwaggerResponse(200, "Removed role", typeof(Response<User>))]
    [SwaggerResponse(404, "User or role not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<User>>> RemoveRole(int userId, int roleId)
    {
        try
        {
            return Ok(await _userRepository.RemoveRole(userId, roleId, Request));
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
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion

    #region Delete

    [HttpDelete("{userId:int}")]
    // todo auth
    [SwaggerResponse(200, "Removed user", typeof(Response<User>))]
    [SwaggerResponse(404, "User not found")]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<Response<User>>> DeleteUser(int userId)
    {
        try
        {
            return Ok(await _userRepository.DeleteUser(userId, Request));
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
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }

    #endregion
}