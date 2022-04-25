using Amazon.S3;
using Amazon.S3.Model;
using ZDC.Server.Services.Interfaces;

namespace ZDC.Server.Services;

public class SpacesService : ISpacesService
{
    private readonly AmazonS3Client _client;

    public SpacesService(IConfiguration configuration)
    {
        _client = new AmazonS3Client(
            configuration.GetValue<string>("SpacesKey"),
            configuration.GetValue<string>("SpacesSecret"),
            new AmazonS3Config
            {
                ServiceURL = "https://nyc3.digitaloceanspaces.com"
            });
    }

    public async Task<string> UploadFile(string file, string fileName, string type)
    {
        var request = new PutObjectRequest
        {
            BucketName = "vzdc",
            Key = $"{type}/{fileName}",
            ContentBody = file,
            CannedACL = S3CannedACL.PublicRead
        };
        await _client.PutObjectAsync(request);
        return $"https://vzdc-k8s.nyc3.digitaloceanspaces.com/{type}/{fileName}";
    }
}
