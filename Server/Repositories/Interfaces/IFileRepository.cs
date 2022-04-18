using ZDC.Shared.Dtos;
using File = ZDC.Shared.Models.File;

namespace ZDC.Server.Repositories.Interfaces;

public interface IFileRepository
{
    /// <summary>
    /// Create file
    /// </summary>
    /// <param name="file">File to create</param>
    /// <param name="request">Raw http request</param>
    /// <returns>Created file</returns>
    Task<Response<File>> CreateFile(File file, HttpRequest request);

    /// <summary>
    /// Get files
    /// </summary>
    /// <param name="request">Raw http request</param>
    /// <returns>Files</returns>
    Task<Response<IList<File>>> GetFiles(HttpRequest request);

    /// <summary>
    /// Get file by id
    /// </summary>
    /// <param name="fileId">File id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.FileNotFoundException">File not found</exception>
    /// <returns>File if found</returns>
    Task<Response<File>> GetFile(int fileId, HttpRequest request);

    /// <summary>
    /// Update file
    /// </summary>
    /// <param name="file">Updated file</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.FileNotFoundException">File not found</exception>
    /// <returns>Updated file</returns>
    Task<Response<File>> UpdateFile(File file, HttpRequest request);

    /// <summary>
    /// Delete file
    /// </summary>
    /// <param name="fileId">File id</param>
    /// <param name="request">Raw http request</param>
    /// <exception cref="Shared.FileNotFoundException">File not found</exception>
    /// <returns>Deleted file</returns>
    Task<Response<File>> DeleteFile(int fileId, HttpRequest request);
}
