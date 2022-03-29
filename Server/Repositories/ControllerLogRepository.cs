using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class ControllerLogRepository : IControllerLogRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public ControllerLogRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    #region Read

    public async Task<Response<IList<ControllerLogDto>>> GetUserControllerLogs(int id)
    {
        if (!_context.Users.Any(x => x.Id == id))
            throw new UserNotFoundException($"User '{id}' not found");
        var logs = await _context.ControllerLogs.Where(x => x.UserId == id).ToListAsync();
        return new Response<IList<ControllerLogDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {logs.Count} controller logs for user '{id}'",
            Data = _mapper.Map<IList<ControllerLog>, IList<ControllerLogDto>>(logs)
        };
    }

    #endregion
}