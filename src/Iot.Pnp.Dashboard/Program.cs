using Iot.PnpDashboard.Configuration;
using Iot.PnpDashboard.Devices;
using Iot.PnpDashboard.Events;
using Iot.PnpDashboard.EventBroadcast;
using Iot.PnpDashboard.Infrastructure;
using Azure.Identity;
using Microsoft.Azure.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR( options=> options.EnableDetailedErrors = true )
    .AddAzureSignalR(option =>
    {
        if (builder.Configuration.GetValue<bool>("Azure:ManagedIdentity:Enabled"))
        {
            DefaultAzureCredentialOptions options = new DefaultAzureCredentialOptions()
            {
                ManagedIdentityClientId = builder.Configuration.GetValue<string>("Azure:ManagedIdentity:ClientId") ?? null
            };
            var credential = new DefaultAzureCredential(options);
            option.Endpoints = new ServiceEndpoint[]
            {
                new ServiceEndpoint(new Uri($"https://{builder.Configuration.GetValue<string>("Azure:SignalR:HostName")}"), credential)
            };
            options.Diagnostics.IsLoggingEnabled = true;
        }
        else
        {
            option.Endpoints = new ServiceEndpoint[]
            {
                new ServiceEndpoint(builder.Configuration.GetValue<string>("Azure:SignalR:ConnectionString"))
            };
        }
    });

builder.Services.AddSingleton<AppConfiguration>();
builder.Services.AddSingleton<RedisConnectionFactory>();
builder.Services.AddSingleton<OnlineDevicesService>();
builder.Services.AddSingleton<IDeviceService, DeviceService>();
builder.Services.AddHostedService<EventHubProcessorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapHub<IotEventsHub>(IotEventsHub.HubUrl);

app.Run();