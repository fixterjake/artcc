using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class SoloCertRepository : ISoloCertRepository
{
    private readonly DatabaseContext _context;
    private readonly ILoggingService _loggingService;
    private readonly IVatusaService _vatusaService;
    private readonly IMapper _mapper;

    public SoloCertRepository(DatabaseContext context, ILoggingService loggingService,
        IVatusaService vatusaService, IMapper mapper)
    {
        _context = context;
        _loggingService = loggingService;
        _vatusaService = vatusaService;
        _mapper = mapper;
    }

    #region Create

    /// <inheritdoc />
    public async Task<Response<SoloCert>> CreateSoloCert(SoloCert soloCert, HttpRequest request)
    {
        var user = await _context.Users.FindAsync(soloCert.UserId) ??
            throw new UserNotFoundException($"User '{soloCert.UserId}' not found");
        if (await _context.Users.FindAsync(soloCert.SubmitterId) == null)
            throw new UserNotFoundException($"User '{soloCert.SubmitterId}' not found");
        if (await _context.SoloCerts.AnyAsync(x => x.UserId == user.Id))
            throw new SoloCertExistsException("User already has a solo cert");

        var oldCert = 0;
        switch (soloCert.Position)
        {
            case SoloCertFacility.Minor:
                oldCert = (int)user.Minor;
                break;
            case SoloCertFacility.Iad:
                oldCert = (int)user.Iad;
                break;
            case SoloCertFacility.Bwi:
                oldCert = (int)user.Bwi;
                break;
            case SoloCertFacility.Dca:
                oldCert = (int)user.Dca;
                break;
            case SoloCertFacility.Dc:
                oldCert = (int)user.Center;
                break;
        }

        soloCert.OldCert = oldCert;

        var result = await _context.SoloCerts.AddAsync(soloCert);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Created solo cert '{result.Entity.Id}'", string.Empty, newData);

        if (soloCert.Position == SoloCertFacility.Dc || soloCert.Cert == 5)
            await _vatusaService.AddSoloCert(soloCert);

        return new Response<SoloCert>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created solo cert '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Read

    /// <inheritdoc />
    public async Task<Response<IList<SoloCertDto>>> GetSoloCerts()
    {
        var soloCerts = await _context.SoloCerts
            .Where(x => x.End >= DateTimeOffset.UtcNow)
            .ToListAsync();
        return new Response<IList<SoloCertDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {soloCerts.Count} solo certs",
            Data = _mapper.Map<IList<SoloCert>, IList<SoloCertDto>>(soloCerts)
        };
    }

    /// <inheritdoc />
    public async Task<Response<SoloCert>> GetSoloCert(int userId)
    {
        var user = await _context.Users.FindAsync(userId) ??
            throw new UserNotFoundException("User not found");

        var cert = await _context.SoloCerts.FirstOrDefaultAsync(x => x.UserId == user.Id) ??
            throw new SoloCertNotFoundException("Solo cert not found");

        return new Response<SoloCert>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Found solo cert '{cert.Id}'",
            Data = cert
        };
    }

    #endregion

    #region Update

    /// <inheritdoc />
    public async Task<Response<SoloCert>> ExtendSoloCert(int soloCertId, DateTimeOffset newEnd, HttpRequest request)
    {
        var cert = await _context.SoloCerts.FindAsync(soloCertId) ??
            throw new SoloCertNotFoundException($"Solo cert '{soloCertId}' not found");

        var oldData = JsonConvert.SerializeObject(cert);
        cert.End = newEnd.ToUniversalTime();
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(cert);

        await _loggingService.AddWebsiteLog(request, $"Updated solo cert '{soloCertId}'", oldData, newData);

        if (cert.Position == SoloCertFacility.Dc || cert.Cert == 5)
        {
            await _vatusaService.DeleteSoloCert(cert);
            await _vatusaService.AddSoloCert(cert);
        }

        return new Response<SoloCert>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Updated solo cert '{soloCertId}'",
            Data = cert
        };
    }

    #endregion

    #region Delete

    /// <inheritdoc />
    public async Task<Response<SoloCert>> DeleteSoloCert(int soloCertId, HttpRequest request)
    {
        var cert = await _context.SoloCerts.FindAsync(soloCertId) ??
            throw new SoloCertNotFoundException($"Solo cert '{soloCertId}' not found");

        var oldData = JsonConvert.SerializeObject(cert);
        _context.SoloCerts.Remove(cert);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted solo cert '{soloCertId}'", oldData, string.Empty);


        if (cert.Position == SoloCertFacility.Dc || cert.Cert == 5)
            await _vatusaService.DeleteSoloCert(cert);

        return new Response<SoloCert>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Deleted solo cert '{soloCertId}'",
            Data = cert
        };
    }

    #endregion
}
