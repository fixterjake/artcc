using ZDC.Shared.Dtos;
using File = ZDC.Shared.Models.File;

namespace ZDC.Server.Repositories.Interfaces;

public interface IFileRepository
{
    Task<Response<File>> CreateFile(File file, HttpRequest request);
    Task<Response<IList<File>>> GetFiles(HttpRequest request);
    Task<Response<File>> GetFile(int fileId, HttpRequest request);
    Task<Response<File>> UpdateFile(File file, HttpRequest request);
    Task<Response<File>> DeleteFile(int fileId, HttpRequest request);
}
