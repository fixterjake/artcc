namespace ZDC.Server.Services.Interfaces;

public interface ISpacesService
{
    Task<string> UploadFile(string file, string fileName, string type);
}
