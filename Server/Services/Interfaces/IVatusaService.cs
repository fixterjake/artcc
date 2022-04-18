using ZDC.Shared.Models;

namespace ZDC.Server.Services.Interfaces;

public interface IVatusaService
{
    Task AddSoloCert(SoloCert soloCert);
    Task DeleteSoloCert(SoloCert soloCert);
    Task AddTrainingTicket(TrainingTicket trainingTicket);
}
