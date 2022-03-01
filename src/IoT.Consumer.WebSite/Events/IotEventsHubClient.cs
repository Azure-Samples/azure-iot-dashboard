using IoT.Consumer.WebSite.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.Devices.Client;
using Microsoft.Rest.TransientFaultHandling;
using System.Data;
using System.Diagnostics;

namespace IoT.Consumer.WebSite.Events
{
    public class IotEventsHubClient : IAsyncDisposable
    {
        //TODO: Message Pack?
        private readonly ILogger _logger;
        private HubConnection? _hubConnection;

        private string? _currentSubscription;
        
        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public IotEventsHubClient(ILogger<IotEventsHubClient> logger, string baseUrl)
        {
            _logger = logger;
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(baseUrl.TrimEnd('/') + IotEventsHub.HubUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.Reconnecting += error =>
            {
                Debug.Assert(_hubConnection.State == HubConnectionState.Reconnecting);

                // Notify users the connection was lost and the client is reconnecting.
                // Start queuing or dropping messages.

                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += connectionId =>
            {
                Debug.Assert(_hubConnection.State == HubConnectionState.Connected);

                // Notify users the connection was reestablished.
                // Start dequeuing messages queued while reconnecting if any.

                return Task.CompletedTask;
            };
        }

        public async Task SubscribeTelemetryAsync(Action<Event> onEventActionCallback)
        {
            await SubscribeDeviceTelemetryAsync("all-devices", onEventActionCallback);
        }
        public async Task SubscribeDeviceTelemetryAsync(string id, Action<Event> onEventActionCallback)
        {
            if (_hubConnection is not null)
            {
                if (!IsConnected)
                {
                    await _hubConnection.StartAsync();
                }

                await UnsubscribeAsync();

                _currentSubscription = id;
                await _hubConnection.SendAsync("Subscribe", id);

                _hubConnection.On<Event>("DeviceTelemetry", onEventActionCallback);
            }
        }

        private async Task UnsubscribeAsync()
        {
            if (_hubConnection is not null && IsConnected && !String.IsNullOrWhiteSpace(_currentSubscription))
            {
                await _hubConnection.SendAsync("Unsubscribe", _currentSubscription);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await UnsubscribeAsync();
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
