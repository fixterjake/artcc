using Microsoft.EntityFrameworkCore;
using Sentry;
using Serilog;
using ZDC.Jobs;
using ZDC.Jobs.Services;
using ZDC.Jobs.Services.Interfaces;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(options =>
    {
        options.ClearProviders();
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        options.AddSerilog(logger, dispose: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<DatabaseContext>(options =>
            options.UseNpgsql(hostContext.Configuration.GetValue<string>("ConnectionString")));
        services.AddScoped<ILoggingService, LoggingService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IDatafeedService, DatafeedService>();
        services.AddScoped<IVatusaService, VatusaService>();
        services.AddScoped<IJobsService, JobsService>();
        services.AddScoped<IAvwxService, AvwxService>();
    })
    .Build();

var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;
var jobs = services.GetRequiredService<IJobsService>();
jobs.AddDatafeedJob(TimeSpan.Zero, 15);
jobs.AddRosterJob(TimeSpan.FromSeconds(10), 15);
jobs.AddAirportsJob(TimeSpan.FromSeconds(20), 5);
jobs.AddEventEmailsJob(TimeSpan.FromSeconds(30), 30);
jobs.StartJobs();


using (SentrySdk.Init(options =>
{
    options.Dsn = scope.ServiceProvider.GetRequiredService<IConfiguration>().GetValue<string>("SentryDsn");
    options.TracesSampleRate = 1.0;
}))
{
    try
    {
        await host.RunAsync();
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine(ex.Message);
    }
}
