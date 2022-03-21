using ZDC.Jobs.Models;

namespace ZDC.Jobs.Services.Interfaces;

public interface IAvwxService
{
    Task<Weather?> GetWeather(string icao);
}
