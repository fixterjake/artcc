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

public class EventService
{
    private readonly HttpClient _client;
    private readonly NavigationManager _navigationManager;

    public EventService(HttpClient client, NavigationManager navigationManager)
    {
        _client = client;
        _navigationManager = navigationManager;
    }

    public async Task<IList<Event>?> GetEvents()
    {
        var response = await _client.GetAsync("/api/Events?skip=0&take=3");
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
            return JsonConvert.DeserializeObject<Response<IList<Event>>>(content)?.Data;
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var responseBad = JsonConvert.DeserializeObject<Response<Guid>>(content);
            _navigationManager.NavigateTo($"/Error/{responseBad?.Data}");
            return null;
        }
        return null;
    }
}
