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
    }

    public async Task AddTrainingTicket(TrainingTicket trainingTicket)
    {
        using var client = new HttpClient();
        var duration = trainingTicket.Start - trainingTicket.End;
        var formContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("instructor_id", trainingTicket.TrainerId.ToString()),
            new KeyValuePair<string, string>("session_date", trainingTicket.Start.ToString("yyyy-mm-dd HH:mm")),
            new KeyValuePair<string, string>("position", trainingTicket.PositionFull),
            new KeyValuePair<string, string>("duration", $"{duration.Hours}:{duration.Minutes}"),
            new KeyValuePair<string, string>("notes", trainingTicket.Comments),
            new KeyValuePair<string, string>("location", trainingTicket.Location.ToString()),
        });
        var response = await client.PostAsync(
            $"{_configuration.GetValue<string>("VatusaUrl")}/user/{trainingTicket.UserId}/training/record?apikey={_configuration.GetValue<string>("VatusaApiKey")}",
            formContent
        );
        Console.WriteLine(response);
    }

}
