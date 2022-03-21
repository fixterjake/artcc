using ZDC.Jobs.Models;

namespace ZDC.Jobs.Services.Interfaces;

public interface IDatafeedService
{
    Task<Datafeed?> GetDatafeed();
    Task<IList<Pilot>> GetZdcPilots(string icao);
    Task<IList<Controller>> GetZdcControllers(Datafeed datafeed);
}
