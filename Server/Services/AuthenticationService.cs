using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VATSIM.Connect.AspNetCore.Server.Services;
using VATSIM.Connect.AspNetCore.Shared.DTO;
using ZDC.Server.Data;
using ZDC.Shared.Models;

namespace ZDC.Server.Services;

public class AuthenticationService : IVatsimAuthenticationService
{

    private readonly DatabaseContext _context;

    public AuthenticationService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Claim>> ProcessVatsimUserLogin(VatsimUserDto user)
    {
        var claims = new List<Claim>();
        var u = await _context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == user.Cid);
        if (u == null || u.Status == UserStatus.Removed)
        {
            claims.Add(new Claim("Rating", $"{user.VatsimDetails.ControllerRating}"));
            claims.Add(new Claim("IsMember", $"{false}"));
            claims.Add(new Claim("CanSignupEvents", $"{false}"));
            claims.Add(new Claim("CanSignupTraining", $"{false}"));
            return claims;
        }

        claims.Add(new Claim("IsMember", $"{true}"));
        claims.Add(new Claim("Rating", $"{u.Rating}"));
        claims.Add(new Claim("CanSignupEvents", $"{u.CanEvents == Access.Yes}"));
        claims.Add(new Claim("CanSignupTraining", $"{u.CanTraining == Access.Yes}"));

        if (u.Roles == null) return claims;
        claims.AddRange(u.Roles
            .Select(_ =>
                new Claim(ClaimTypes.Role, _.Name)
            ));

        return claims;
    }
}
