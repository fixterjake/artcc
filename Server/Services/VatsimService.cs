using System.Net.Http.Headers;
using ZDC.Server.Services.Interfaces;
using ZDC.Server.Services.Responses;
using ZDC.Shared.Models;

namespace ZDC.Server.Services;

public class VatsimService : IVatsimService
{
    private readonly IConfiguration _configuration;

    public VatsimService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<double> GetUserRatingHours(int userId, Rating rating)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Token", _configuration.GetValue<string>("VatsimApiKey"));

        var vatsimApiUrl = _configuration.GetValue<string>("VatsimApiUrl");
        var response = await client.GetFromJsonAsync<RatingHours>($"{vatsimApiUrl}/ratings/{userId}/rating_times");
        if (response == null)
            return 0;

        return rating switch
        {
            Rating.INAC => 0,
            Rating.SUS => 0,
            Rating.OBS => 0,
            Rating.S1 => response.S1,
            Rating.S2 => response.S2,
            Rating.S3 => response.S3,
            Rating.C1 => response.C1,
            Rating.C2 => response.C2,
            Rating.C3 => response.C3,
            Rating.I1 => response.I1,
            Rating.I2 => response.I2,
            Rating.I3 => response.I3,
            Rating.SUP => response.Sup,
            Rating.ADM => response.Adm,
            _ => 0,
        };
    }

    public async Task<DateTime> GetLastRatingChange(int userId)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Token", _configuration.GetValue<string>("VatsimApiKey"));

        var vatsimApiUrl = _configuration.GetValue<string>("VatsimApiUrl");
        var response = await client.GetFromJsonAsync<Ratings>($"{vatsimApiUrl}/ratings/{userId}");
        if (response == null)
            return DateTime.UtcNow;
        return DateTime.Parse(response.LastRatingChange);
    }
}
