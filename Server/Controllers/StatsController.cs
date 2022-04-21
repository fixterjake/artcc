using Microsoft.AspNetCore.Mvc;
using Sentry;
using Swashbuckle.AspNetCore.Annotations;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Shared.Dtos;

namespace ZDC.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly IStatsRepository _statsRepository;
    private readonly IHub _sentryHub;

    public StatsController(IStatsRepository statsRepository, IHub sentryHub)
    {
        _statsRepository = statsRepository;
        _sentryHub = sentryHub;
    }

    [HttpGet]
    [SwaggerResponse(200, "Got stats", typeof(Response<IList<StatsDto>>))]
    [SwaggerResponse(400, "An error occurred")]
    public async Task<ActionResult<IList<StatsDto>>> GetStats(int month = 0, int year = 0)
    {
        try
        {
            return Ok(await _statsRepository.GetStats(month, year));
        }
        catch (Exception ex)
        {
            return _sentryHub.CaptureException(ex).ReturnActionResult();
        }
    }
}
