using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Prometheus;
using Prometheus.SystemMetrics;
using Serilog;
using VATSIM.Connect.AspNetCore.Server.Extensions;
using ZDC.Server.Data;
using ZDC.Server.Repositories;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services;
using ZDC.Server.Services.Interfaces;
using ZDC.Server.Validators;
using ZDC.Shared;
using ZDC.Shared.Extensions;
using ZDC.Shared.Models;
using File = ZDC.Shared.Models.File;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry(options =>
{
    options.TracesSampleRate = 1.0;
});

builder.Logging.ClearProviders();
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Logging.AddSerilog(logger, dispose: true);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSwaggerGen(c =>
    {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Format is the word Bearer, then a space, followed by the token",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    }
);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("Redis").GetValue<string>("Host");
    options.InstanceName = "vzdc_api";
});
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetValue<string>("ConnectionString")));

builder.Services.AddVatsimConnect<AuthenticationService>(builder.Configuration.GetSection("VatsimServerOptions"));
builder.Services.AddAuthorization(options => { options.AddWashingtonArtccPolicies(); });

builder.Services.AddScoped<IValidator<Airport>, AirportValidator>();
builder.Services.AddScoped<IValidator<Announcement>, AnnouncementValidator>();
builder.Services.AddScoped<IValidator<Comment>, CommentValidator>();
builder.Services.AddScoped<IValidator<EventPosition>, EventPositionValidator>();
builder.Services.AddScoped<IValidator<EventRegistration>, EventRegistrationValidator>();
builder.Services.AddScoped<IValidator<Event>, EventValidator>();
builder.Services.AddScoped<IValidator<Feedback>, FeedbackValidator>();
builder.Services.AddScoped<IValidator<File>, FileValidator>();
builder.Services.AddScoped<IValidator<Loa>, LoaValidator>();
builder.Services.AddScoped<IValidator<Ots>, OtsValidator>();
builder.Services.AddScoped<IValidator<Position>, PositionValidator>();
builder.Services.AddScoped<IValidator<Settings>, SettingsValidator>();
builder.Services.AddScoped<IValidator<SoloCert>, SoloCertValidator>();
builder.Services.AddScoped<IValidator<StaffingRequest>, StaffingRequestValidator>();
builder.Services.AddScoped<IValidator<TrainingTicket>, TrainingTicketValidator>();
builder.Services.AddScoped<IValidator<VisitRequest>, VisitRequestValidator>();

builder.Services.AddAutoMapper(typeof(AutomapperConfig));

builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVatusaService, VatusaService>();
builder.Services.AddScoped<IVatsimService, VatsimService>();
builder.Services.AddScoped<ISpacesService, SpacesService>();

builder.Services.AddScoped<IAirportRepository, AirportRepository>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IControllerLogRepository, ControllerLogRepository>();
builder.Services.AddScoped<IEmailLogRepository, EmailLogRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IStatsRepository, StatsRepository>();
builder.Services.AddScoped<ILoaRepository, LoaRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IOnlineControllerRepository, OnlineControllerRepository>();
builder.Services.AddScoped<IOtsRepository, OtsRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<ISoloCertRepository, SoloCertRepository>();
builder.Services.AddScoped<IStaffingRequestRepository, StaffingRequestRepository>();
builder.Services.AddScoped<ITrainingTicketRepository, TrainingTicketRepository>();
builder.Services.AddScoped<IUploadRepository, UploadRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVisitRequestRepository, VisitRequestRepository>();
builder.Services.AddScoped<IWarningRepository, WarningRepository>();

builder.Services.AddSystemMetrics();

var app = builder.Build();

app.UseSentryTracing();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    using var context = serviceScope.ServiceProvider.GetService<DatabaseContext>();
    if (context != null && context.Database.GetMigrations().Any()) context.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                       ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();


app.UseMetricServer();
app.UseHttpMetrics();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
