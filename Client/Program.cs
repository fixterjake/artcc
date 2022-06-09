using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using VATSIM.Connect.AspNetCore.Blazor.Client.Extensions;
using ZDC.Client;
using ZDC.Client.Services;
using ZDC.Shared.Extensions;
using ZDC.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<SpinnerService>();
builder.Services
    .AddBlazorise()
    .AddBootstrapProviders()
    .AddBootstrapComponents()
    .AddFontAwesomeIcons();

builder.Services.AddVatsimConnect();
builder.Services.AddAuthorizationCore(options => { options.AddWashingtonArtccPolicies(); });

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddTransient<OnlineControllerService>();
builder.Services.AddTransient<EventService>();

await builder.Build().RunAsync();
