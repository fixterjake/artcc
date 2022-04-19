using System.Net.Http.Headers;
using System.Net.Http.Json;
using ZDC.Jobs.Models;
using ZDC.Jobs.Services.Interfaces;

namespace ZDC.Jobs.Services;

public class AvwxService : IAvwxService
{
    private readonly DatabaseContext _context;
    private readonly IConfiguration _configuration;

    public AvwxService(DatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<Weather?> GetWeather(string icao)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Token", _configuration.GetValue<string>("AvwxApiKey"));

        return await client.GetFromJsonAsync<Weather>($"{_configuration.GetValue<string>("AvwxUrl")}/{icao}");
    }
}
