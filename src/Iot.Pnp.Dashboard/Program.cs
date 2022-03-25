using Iot.PnpDashboard.Configuration;
using Iot.PnpDashboard.Devices;
using Iot.PnpDashboard.Events;
using Iot.PnpDashboard.EventBroadcast;
using Iot.PnpDashboard.Infrastructure;
using Azure.Identity;
using Microsoft.Azure.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR( options=> options.EnableDetailedErrors = true )
    .AddAzureSignalR();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
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