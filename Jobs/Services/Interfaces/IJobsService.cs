namespace ZDC.Jobs.Services.Interfaces;

public interface IJobsService
{
    void StartJobs();
    void AddDatafeedJob(TimeSpan startAfter, int seconds);
    void AddRosterJob(TimeSpan startAfter, int minutes);
    void AddAirportsJob(TimeSpan startAfter, int minutes);
    void AddEventEmailsJob(TimeSpan startAfter, int minutes);
    void AddSoloCertsJob(TimeSpan startAfter, int minutes);
    void AddLoasJob(TimeSpan startAfter, int minutes);
}
