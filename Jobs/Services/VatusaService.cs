using System.Net.Http.Json;
using ZDC.Jobs.Models;
using ZDC.Jobs.Services.Interfaces;

namespace ZDC.Jobs.Services;

public class VatusaService : IVatusaService
{
    private readonly IConfiguration _config;

    public VatusaService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<Roster?> GetRoster()
    {
        using var client = new HttpClient();
        return await client.GetFromJsonAsync<Roster>(
                $"{_config.GetValue<string>("VatusaRosterUrl")}?apikey={_config.GetValue<string>("VatusaApiKey")}"
            );
    }
}
