using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ZDC.Server.Data;
using ZDC.Shared.Models;

namespace ZDC.Server.Extensions;

public static class HttpContextExtensions
{
    public static int? GetCid(this HttpContext httpContext)
    {
        var cidRaw = httpContext.User.Claims.FirstOrDefault(x => x.Type == "cid")?.Value;
        if (cidRaw == null)
            return null;

        var goodCid = int.TryParse(cidRaw, out var cid);
        return goodCid ? cid : null;
    }

    public static async Task<User?> GetUser(this HttpContext httpContext, DatabaseContext context)
    {
        var cidRaw = httpContext.User.Claims.FirstOrDefault(x => x.Type == "cid")?.Value;
        if (cidRaw == null)
            return null;

        var goodCid = int.TryParse(cidRaw, out var cid);
        if (!goodCid)
            return null;

        var user = await context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == cid);
        return user;
    }

    public static async Task<EventRegistration?> GetUserEventRegistration(this HttpContext httpContext, DatabaseContext context, int eventId)
    {
        var user = await httpContext.GetUser(context);
        if (user == null)
            return null;

        var registration = await context.EventsRegistrations
            .Where(x => x.Id == eventId)
            .FirstOrDefaultAsync(x => x.UserId == user.Id);
        return registration;
    }

    public static async Task<bool> IsStaff(this HttpContext httpContext, DatabaseContext context)
    {
        var user = await httpContext.GetUser(context);
        if (user == null)
            return false;

        return await context.Roles.Select(x => x.Name)
            .AnyAsync(x =>
                x == "ATM" || x == "DATM" || x == "TA" ||
                x == "ATA" || x == "WM" || x == "AWM" ||
                x == "EC" || x == "AEC" || x == "FE" ||
                x == "AFE" || x == "WEB" || x == "EVENTS"
            );
    }

    public static async Task<bool> IsTrainingStaff(this HttpContext httpContext, DatabaseContext context)
    {
        var user = await httpContext.GetUser(context);
        if (user == null)
            return false;

        return await context.Roles.Select(x => x.Name)
            .AnyAsync(x =>
                x == "ATM" || x == "DATM" || x == "TA" ||
                x == "ATA" || x == "WM" || x == "AWM" ||
                x == "INS" || x == "MTR"
            );
    }
}
