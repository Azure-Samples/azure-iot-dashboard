using IoT.Consumer.WebSite.Events;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Azure.IoT.ModelsRepository;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Microsoft.Rest;
using Microsoft.Azure.Devices.Serialization;
using IoT.Consumer.WebSite.SignalR;

namespace IoT.Consumer.WebSite.Devices
{
    public class OnlineDevicesService: IAsyncDisposable
    {
        public List<Device> Devices { get; } = new List<Device>();

        private readonly ILogger _logger;
        private readonly IotEventsHubClient _eventsHubClient;


        public OnlineDevicesService(string baseUri, IConfiguration confiuration, ILogger<DeviceService> logger)
        {
            _logger = logger;
            _eventsHubClient = new IotEventsHubClient(LoggerFactory.Create(options => { }).CreateLogger<IotEventsHubClient>(), baseUri);
            _eventsHubClient.Subscribe(UpsertOnlineDevices);
        }

        public async Task Subscribe()
        {
            await _eventsHubClient.SubscribeTelemetryAsync();
        }

        private void UpsertOnlineDevices(Event e)
        {
            int idx = Devices.FindIndex(d => d.DeviceId == e.DeviceId);
            if (idx != -1)
            {
                if (e.DataSchema != null)
                {
                    Devices[idx].ModelId = e.DataSchema;
                }

                if (e.Operation != null)
                {
                    Devices[idx].MessageSource = e.MessageSource;
                    Devices[idx].LastOperation = e.Operation;
                    Devices[idx].LastOperationTimestamp = e.EnqueuedTime;
                    Devices[idx].Disconnected = e.MessageSource == "deviceConnectionStateEvents" && e.Operation == "deviceDisconnected";
                }

                if (e.MessageSource == "Telemetry")
                {                    
                    Devices[idx].LastTelemetryTimestamp = e.EnqueuedTime;
                    Devices[idx].Disconnected = false;
                }
            }
            else
            {
                var device = new Device()
                {
                    DeviceId = e.DeviceId,
                    ModelId = e.DataSchema,
                    MessageSource = e.Operation is not null ? e.MessageSource : null,
                    LastOperation = e.Operation is not null ? e.Operation : null,
                    LastTelemetryTimestamp = e.MessageSource == "Telemetry" ? e.EnqueuedTime : null,
                    LastOperationTimestamp = e.Operation is not null ? e.EnqueuedTime : null,
                    Disconnected = e.MessageSource == "deviceConnectionStateEvents" && e.Operation == "deviceDisconnected"
                };
                Devices.Add(device);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _eventsHubClient.DisposeAsync();
        }
    }
}
