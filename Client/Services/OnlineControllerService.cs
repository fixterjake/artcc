using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ZDC.Shared.Dtos;
using ZDC.Shared.Models;

namespace ZDC.Client.Services;

public class OnlineControllerService
{
    private readonly HttpClient _client;
    private readonly NavigationManager _navigationManager;

    public OnlineControllerService(HttpClient client, NavigationManager navigationManager)
    {
        _client = client;
        _navigationManager = navigationManager;
    }

    public async Task<IList<OnlineController>?> GetOnlineControllers()
    {
        var response = await _client.GetAsync("/api/OnlineControllers");
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
            return JsonConvert.DeserializeObject<Response<IList<OnlineController>>>(content)?.Data;
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var responseBad = JsonConvert.DeserializeObject<Response<Guid>>(content);
            _navigationManager.NavigateTo($"/Error/{responseBad?.Data}");
            return null;
        }
        return null;
    }
}
