using Quartz;
using Quartz.Impl;
using ZDC.Jobs.Jobs;
using ZDC.Jobs.Services.Interfaces;

namespace ZDC.Jobs.Services;

public class JobsService : IJobsService
{
    private readonly IScheduler _scheduler;

    public JobsService(ILogger<JobsService> logger, DatabaseContext context, IVatusaService vatusaService, IConfiguration configuration,
        IDatafeedService datafeedService, IAvwxService avwxService, ILoggingService loggingService, IEmailService emailService)
    {
        var factory = new StdSchedulerFactory();
        _scheduler = factory.GetScheduler().Result;
        _scheduler.Context.Put("Logger", logger);
        _scheduler.Context.Put("DatabaseContext", context);
        _scheduler.Context.Put("VatusaService", vatusaService);
        _scheduler.Context.Put("Configuration", configuration);
        _scheduler.Context.Put("DatafeedService", datafeedService);
        _scheduler.Context.Put("AvwxService", avwxService);
        _scheduler.Context.Put("LoggingService", loggingService);
        _scheduler.Context.Put("EmailService", emailService);
    }

    public async void StartJobs()
    {
        await _scheduler.Start();
    }

    public async void AddDatafeedJob(TimeSpan startAfter, int seconds)
    {
        var job = JobBuilder.Create<DatafeedJob>()
            .WithIdentity("DatafeedJob", "Jobs")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("DatafeedTrigger", "Jobs")
            .StartAt(DateTime.UtcNow.Add(startAfter))
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(seconds)
                .RepeatForever())
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
    }

    public async void AddRosterJob(TimeSpan startAfter, int minutes)
    {
        var job = JobBuilder.Create<RosterJob>()
            .WithIdentity("RosterJob", "Jobs")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("RosterTrigger", "Jobs")
            .StartAt(DateTime.UtcNow.Add(startAfter))
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(minutes)
                .RepeatForever())
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
    }

    public async void AddAirportsJob(TimeSpan startAfter, int minutes)
    {
        var job = JobBuilder.Create<AirportsJob>()
            .WithIdentity("AirportsJob", "Jobs")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("AirportsTrigger", "Jobs")
            .StartAt(DateTime.UtcNow.Add(startAfter))
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(minutes)
                .RepeatForever())
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
    }

    public async void AddEventEmailsJob(TimeSpan startAfter, int minutes)
    {
        var job = JobBuilder.Create<EventEmailsJob>()
            .WithIdentity("EventEmailsJob", "Jobs")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("EventEmailsTrigger", "Jobs")
            .StartAt(DateTime.UtcNow.Add(startAfter))
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(minutes)
                .RepeatForever())
            .Build();

        await _scheduler.ScheduleJob(job, trigger);
    }

    public async void AddSoloCertsJob(TimeSpan startAfter, int minutes)
    {
        throw new NotImplementedException();
    }

    public async void AddLoasJob(TimeSpan startAfter, int minutes)
    {
        throw new NotImplementedException();
    }
}
