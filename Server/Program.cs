using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Prometheus;
using Prometheus.SystemMetrics;
using Serilog;
using ZDC.Server.Data;
using ZDC.Server.Repositories;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services;
using ZDC.Server.Services.Interfaces;
using ZDC.Server.Validators;
using ZDC.Shared;
using ZDC.Shared.Models;
using File = ZDC.Shared.Models.File;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetValue<string>("ConnectionString")));

builder.Services.AddTransient<IValidator<Airport>, AirportValidator>();
builder.Services.AddTransient<IValidator<Comment>, CommentValidator>();
builder.Services.AddTransient<IValidator<EventPosition>, EventPositionValidator>();
builder.Services.AddTransient<IValidator<EventRegistration>, EventRegistrationValidator>();
builder.Services.AddTransient<IValidator<Event>, EventValidator>();
builder.Services.AddTransient<IValidator<Feedback>, FeedbackValidator>();
builder.Services.AddTransient<IValidator<File>, FileValidator>();
builder.Services.AddTransient<IValidator<Loa>, LoaValidator>();
builder.Services.AddTransient<IValidator<News>, NewsValidator>();
builder.Services.AddTransient<IValidator<Position>, PositionValidator>();
builder.Services.AddTransient<IValidator<Settings>, SettingsValidator>();
builder.Services.AddTransient<IValidator<SoloCert>, SoloCertValidator>();
builder.Services.AddTransient<IValidator<StaffingRequest>, StaffingRequestValidator>();
builder.Services.AddTransient<IValidator<TrainingTicket>, TrainingTicketValidator>();
builder.Services.AddTransient<IValidator<VisitRequest>, VisitRequestValidator>();

builder.Services.AddAutoMapper(typeof(AutomapperConfig));

builder.Services.AddTransient<ILoggingService, LoggingService>();
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IAirportRepository, AirportRepository>();
builder.Services.AddTransient<ICommentRepository, CommentRepository>();
builder.Services.AddTransient<IControllerLogRepository, ControllerLogRepository>();
builder.Services.AddTransient<IDebugLogRepository, DebugLogRepository>();
builder.Services.AddTransient<IEmailLogRepository, EmailLogRepository>();

builder.Services.AddSystemMetrics();

var app = builder.Build();

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


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
