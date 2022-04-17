using Newtonsoft.Json.Linq;
using RestSharp;
using ZDC.Server.Services.Interfaces;
using ZDC.Shared.Models;

namespace ZDC.Server.Services;

public class VatusaService : IVatusaService
{
    private readonly IConfiguration _configuration;

    public VatusaService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task AddSoloCert(SoloCert soloCert)
    {
        using var client = new HttpClient();
        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("cid", soloCert.UserId.ToString()),
            new KeyValuePair<string, string>("position", soloCert.PositionString),
            new KeyValuePair<string, string>("expDate", soloCert.End.ToString("yyyy-MM-dd"))
        });
        await client.PostAsync(
            $"{_configuration.GetValue<string>("VatusaUrl")}/solo?apikey={_configuration.GetValue<string>("VatusaApiKey")}",
            formContent
        );
    }

    public async Task DeleteSoloCert(SoloCert soloCert)
    {
        using var client = new RestClient(
                $"{_configuration.GetValue<string>("VatusaUrl")}/solo?apikey={_configuration.GetValue<string>("VatusaApiKey")}"
            );
        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("cid", soloCert.UserId.ToString()),
            new KeyValuePair<string, string>("position", soloCert.PositionString),
        });

        var request = new RestRequest()
            .AddBody(formContent);

        var response = await client.DeleteAsync(request);
        Console.WriteLine(response);
    }
}
