using Microsoft.EntityFrameworkCore;
using ZDC.Shared.Models;
using File = ZDC.Shared.Models.File;

namespace ZDC.Server.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

#nullable disable

    public DbSet<Airport> Airports { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<ControllerLog> ControllerLogs { get; set; }
    public DbSet<DebugLog> DebugLogs { get; set; }
    public DbSet<EmailLog> EmailLogs { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventPosition> EventsPositions { get; set; }
    public DbSet<EventRegistration> EventsRegistrations { get; set; }
    public DbSet<Feedback> Feedback { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<Hours> Hours { get; set; }
    public DbSet<Loa> Loas { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<OnlineController> OnlineControllers { get; set; }
    public DbSet<Ots> Ots { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Settings> Settings { get; set; }
    public DbSet<SoloCert> SoloCerts { get; set; }
    public DbSet<StaffingRequest> StaffingRequests { get; set; }
    public DbSet<TrainingTicket> TrainingTickets { get; set; }
    public DbSet<Upload> Uploads { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<VisitRequest> VisitRequests { get; set; }
    public DbSet<Warning> Warnings { get; set; }
    public DbSet<WebsiteLog> WebsiteLogs { get; set; }

#nullable restore
}