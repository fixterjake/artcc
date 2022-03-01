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
using ZDC.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Logging.AddSerilog(logger);

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

builder.Services.AddAutoMapper(typeof(AutomapperConfig));

builder.Services.AddTransient<ILoggingService, LoggingService>();
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IAirportRepository, AirportRepository>();
builder.Services.AddTransient<ICommentRepository, CommentRepository>();
builder.Services.AddTransient<IControllerLogRepository, ControllerLogRepository>();
builder.Services.AddTransient<IDebugLogRepository, DebugLogRepository>();

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
