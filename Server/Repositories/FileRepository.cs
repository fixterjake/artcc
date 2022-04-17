using Amazon.Runtime.Internal.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Net;
using ZDC.Server.Data;
using ZDC.Server.Extensions;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;
using File = ZDC.Shared.Models.File;
using FileNotFoundException = ZDC.Shared.FileNotFoundException;

namespace ZDC.Server.Repositories;

public class FileRepository : IFileRepository
{
    private readonly DatabaseContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILoggingService _loggingService;

    public FileRepository(DatabaseContext context, IDistributedCache cache, ILoggingService loggingService)
    {
        _context = context;
        _cache = cache;
        _loggingService = loggingService;
    }

    #region Create

    public async Task<Response<File>> CreateFile(File file, HttpRequest request)
    {
        var result = await _context.Files.AddAsync(file);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);


        await _loggingService.AddWebsiteLog(request, $"Created file '{result.Entity.Id}'", string.Empty, newData);

        return new Response<File>
        {
            StatusCode = HttpStatusCode.Created,
            Message = $"Created file '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Read

    public async Task<Response<IList<File>>> GetFiles(HttpRequest request)
    {
        var cachedFiles = await _cache.GetStringAsync("_files");
        if (!string.IsNullOrEmpty(cachedFiles))
        {
            var files = JsonConvert.DeserializeObject<List<File>>(cachedFiles);
            if (files != null)
            {
                if (!await request.HttpContext.IsStaff(_context))
                    files = files.Where(x => x.Category != FileCategory.Staff).ToList();
                if (!await request.HttpContext.IsTrainingStaff(_context))
                    files = files.Where(x => x.Category != FileCategory.TrainingStaff).ToList();
                return new Response<IList<File>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = $"Got {files.Count} files",
                    Data = files
                };
            }
        }

        var result = await _context.Files.ToListAsync();
        var expiryOptions = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };
        await _cache.SetStringAsync("_files", JsonConvert.SerializeObject(result), expiryOptions);

        if (!await request.HttpContext.IsStaff(_context))
            result = result.Where(x => x.Category != FileCategory.Staff).ToList();
        if (!await request.HttpContext.IsTrainingStaff(_context))
            result = result.Where(x => x.Category != FileCategory.TrainingStaff).ToList();

        return new Response<IList<File>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got {result.Count} files",
            Data = result
        };
    }


    public async Task<Response<File>> GetFile(int fileId, HttpRequest request)
    {
        var file = await _context.Files.FindAsync(fileId) ??
            throw new FileNotFoundException($"File '{fileId}' not found");
        if (!await request.HttpContext.IsStaff(_context) && file.Category == FileCategory.Staff)
            throw new FileNotFoundException($"File '{fileId}' not found");
        if (!await request.HttpContext.IsTrainingStaff(_context) && file.Category == FileCategory.TrainingStaff)
            throw new FileNotFoundException($"File '{fileId}' not found");

        return new Response<File>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Got file '{file.Id}'",
            Data = file
        };
    }

    #endregion

    #region Update

    public async Task<Response<File>> UpdateFile(File file, HttpRequest request)
    {
        var dbFile = await _context.Files.AsNoTracking().FirstOrDefaultAsync(x => x.Id == file.Id) ??
            throw new FileNotFoundException($"File '{file.Id}' not found");

        var oldData = JsonConvert.SerializeObject(dbFile);
        file.Updated = DateTimeOffset.UtcNow;
        var result = _context.Files.Update(file);
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Updated file '{result.Entity.Id}'", oldData, newData);

        return new Response<File>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Updated file '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion

    #region Delete

    public async Task<Response<File>> DeleteFile(int fileId, HttpRequest request)
    {
        var file = await _context.Files.FindAsync(fileId) ??
            throw new FileNotFoundException($"File '{fileId}' not found");

        var oldData = JsonConvert.SerializeObject(file);
        var result = _context.Files.Remove(file);
        await _context.SaveChangesAsync();

        await _loggingService.AddWebsiteLog(request, $"Deleted file '{result.Entity.Id}'", oldData, string.Empty);

        return new Response<File>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Deleted file '{result.Entity.Id}'",
            Data = result.Entity
        };
    }

    #endregion
}
