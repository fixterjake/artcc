using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
}
