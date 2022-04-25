using Newtonsoft.Json;
using System.Net;
using ZDC.Server.Data;
using ZDC.Server.Repositories.Interfaces;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories;

public class UploadRepository : IUploadRepository
{
    private readonly DatabaseContext _context;
    private readonly ISpacesService _spacesService;
    private readonly ILoggingService _loggingService;

    public UploadRepository(DatabaseContext context, ISpacesService spacesService, ILoggingService loggingService)
    {
        _context = context;
        _spacesService = spacesService;
        _loggingService = loggingService;
    }

    /// <inheritdoc />
    public async Task<Response<Upload>> AddUpload(string type, HttpRequest request)
    {
        if (!request.Form.Files.Any())
            throw new UploadNotFoundException("No file found");
        var file = request.Form.Files.First();
        var reader = new StreamReader(file.OpenReadStream());

        var url = await _spacesService.UploadFile(await reader.ReadToEndAsync(), file.FileName.Replace(" ", "-"), type);

        var result = await _context.Uploads.AddAsync(new Upload
        {
            Url = url,
            Name = file.FileName
        });
        await _context.SaveChangesAsync();
        var newData = JsonConvert.SerializeObject(result.Entity);

        await _loggingService.AddWebsiteLog(request, $"Added upload '{result.Entity.Id}'", string.Empty, newData);
        return new Response<Upload>
        {
            StatusCode = HttpStatusCode.OK,
            Message = $"Added upload '{result.Entity.Id}'",
            Data = result.Entity
        };
    }
}
