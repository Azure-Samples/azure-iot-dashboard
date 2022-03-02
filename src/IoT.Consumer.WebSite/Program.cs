using IoT.Consumer.WebSite.Devices;
using IoT.Consumer.WebSite.Events;
using IoT.Consumer.WebSite.SignalR;
using Microsoft.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR()
    .AddAzureSignalR();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton<IDeviceService, DeviceService>();

builder.Services.AddHostedService<EventProcessor>();


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
