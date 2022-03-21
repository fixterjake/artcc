using Microsoft.EntityFrameworkCore;
using ZDC.Shared.Models;

namespace ZDC.Jobs;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

#nullable disable

    public DbSet<Airport> Airports { get; set; }
    public DbSet<ControllerLog> ControllerLogs { get; set; }
    public DbSet<DebugLog> DebugLogs { get; set; }
    public DbSet<EmailLog> EmailLogs { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventPosition> EventsPositions { get; set; }
    public DbSet<EventRegistration> EventsRegistrations { get; set; }
    public DbSet<Hours> Hours { get; set; }
    public DbSet<Loa> Loas { get; set; }
    public DbSet<OnlineController> OnlineControllers { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<WebsiteLog> WebsiteLogs { get; set; }

#nullable restore
}