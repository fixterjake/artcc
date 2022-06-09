using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using ZDC.Jobs.Models;
using ZDC.Jobs.Services.Interfaces;

namespace ZDC.Jobs.Services;

public class DatafeedService : IDatafeedService
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;

    public DatafeedService(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<Datafeed?> GetDatafeed()
    {
        try
        {
            using var client = new HttpClient();

            var status = await client.GetFromJsonAsync<Status>(_configuration.GetValue<string>("StatusUrl"));
            var url = status?.Data?.V3?.FirstOrDefault() ?? string.Empty;

            return await client.GetFromJsonAsync<Datafeed>(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task<IList<Controller>> GetZdcControllers(Datafeed datafeed)
    {
        var result = new List<Controller>();
        var positions = await _context.Positions.Select(x => x.Name).ToListAsync();

        if (positions == null || datafeed == null || datafeed.Controllers == null)
            return result;

        foreach (var entry in datafeed.Controllers)
        {
            if (entry.Callsign.Contains("OBS"))
                continue;
            if (positions.Contains(entry.Callsign.Length > 4 ? entry.Callsign[..4] : entry.Callsign))
                result.Add(entry);
        }
        return result;
    }

    public async Task<IList<Pilot>> GetZdcPilots(string icao)
    {
        var result = new List<Pilot>();
        var datafeed = await GetDatafeed();

        if (datafeed == null || datafeed.Pilots == null)
            return result;

        foreach (var entry in datafeed.Pilots)
        {
            if (entry.Flightplan == null)
                continue;
            if (entry.Flightplan.Departure.Equals(icao, StringComparison.OrdinalIgnoreCase) ||
                entry.Flightplan.Arrival.Equals(icao, StringComparison.OrdinalIgnoreCase))
                result.Add(entry);
        }
        return result;
    }
}
