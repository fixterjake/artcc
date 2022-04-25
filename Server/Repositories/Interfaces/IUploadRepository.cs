using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Server.Repositories.Interfaces;

public interface IUploadRepository
{
    /// <summary>
    /// Add an upload to DO spaces
    /// </summary>
    /// <param name="type">Type of upload, will signify what folder to upload to</param>
    /// <param name="request">Raw http request, will contain the image</param>
    /// <exception cref="Shared.UploadNotFoundException">Request did not contain a file</exception>
    /// <returns>Upload object</returns>
    Task<Response<Upload>> AddUpload(string type, HttpRequest request);
}
