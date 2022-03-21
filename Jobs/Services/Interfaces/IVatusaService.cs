using ZDC.Jobs.Models;

namespace ZDC.Jobs.Services.Interfaces;

public interface IVatusaService
{
    Task<Roster?> GetRoster();
}
