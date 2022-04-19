using Quartz;
using Sentry;
using ZDC.Jobs.Services;
using ZDC.Jobs.Services.Interfaces;

namespace ZDC.Jobs.Jobs;

public class AirportsJob : IJob
{

#nullable disable

    private ILogger<JobsService> _logger;
    private DatabaseContext _context;
    private ILoggingService _loggingService;
    private IDatafeedService _datafeedService;
    private IAvwxService _avwxService;

#nullable restore

    public async Task Execute(IJobExecutionContext context)
    {
        var scheduler = context.Scheduler.Context;
        _logger = (ILogger<JobsService>)scheduler.Get("Logger");
        _context = (DatabaseContext)scheduler.Get("DatabaseContext");
        _loggingService = (ILoggingService)scheduler.Get("LoggingService");
        _datafeedService = (IDatafeedService)scheduler.Get("DatafeedService");
        _avwxService = (IAvwxService)scheduler.Get("AvwxService");

        _logger.LogInformation("Running airports job...");

        var start = DateTimeOffset.UtcNow;
        await UpdateAirports();
        var end = DateTimeOffset.UtcNow;

        _logger.LogInformation("Airports job finished -- Took {time} seconds", Math.Round((end - start).TotalSeconds, 2));
    }

    public async Task UpdateAirports()
    {
        try
        {
            foreach (var entry in _context.Airports.ToList())
            {
                var weather = await _avwxService.GetWeather(entry.Icao);
                if (weather == null)
                {
                    _logger.LogError("Weather for {entry} null... continuing.", entry);
                    continue;
                }

                var gust = string.Empty;
                if (weather.WindGust != null)
                    gust = $"Gusting {weather.WindGust.Value}";
                entry.Metar = weather.MetarRaw ?? string.Empty;
                entry.Conditions = weather.FlightRules ?? string.Empty;
                entry.Winds = $"{weather.WindDirection?.Value}° at " +
                              $"{weather.WindSpeed?.Value} {gust}";
                entry.Altimeter = weather.Altimeter?.Value.ToString() ?? string.Empty;

                var pilots = await _datafeedService.GetZdcPilots(entry.Icao);
                if (pilots != null)
                {
                    entry.InboundFlights = pilots.Count(x => x.Flightplan?.Arrival == entry.Icao);
                    entry.OutboundFlights = pilots.Count(x => x.Flightplan?.Departure == entry.Icao);
                }
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }
}
