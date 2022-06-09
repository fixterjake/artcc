using System.Collections.Generic;
using ZDC.Shared.Models;

namespace ZDC.Client.Pages;

public partial class Index
{
    public IList<OnlineController>? OnlineControllers { get; set; }
    public IList<Event>? Event { get; set; }

    protected override async void OnInitialized()
    {
        _spinnerService.Show();

        OnlineControllers = await _onlineControllerService.GetOnlineControllers();
        Event = await _eventService.GetEvents();

        StateHasChanged();

        _spinnerService.Hide();
    }
}
