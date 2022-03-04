using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Iot.PnpDashboard.Events;

namespace Iot.PnpDashboard.SignalR
{
    public class IotEventsHubClient : IObservable<Event>, IAsyncDisposable
    {
        //TODO: Message Pack?
        private readonly ILogger _logger;
        private HubConnection? _hubConnection;
        private List<IObserver<Event>> _eventObservers = new List<IObserver<Event>>();

        //TODO: Podriamos meter otro cojunto de observers por subtipo de evento???
        //private List<IObserver<Event>> _deviceTwinChangeObservers = new List<IObserver<Event>>();
        //private List<IObserver<Event>> _deviceConnectionStateObservers = new List<IObserver<Event>>();


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

        public IDisposable Subscribe(IObserver<Event> observer)
        {
            if (!_eventObservers.Contains(observer))
                _eventObservers.Add(observer);

            return new Unsubscriber<Event>(_eventObservers, observer);
        }

        public async Task SubscribeTelemetryAsync()
        {
            await SubscribeTelemetryAsync("all-devices");
        }
        public async Task SubscribeTelemetryAsync(string id)
        {
            if (_hubConnection is not null)
            {
                if (!IsConnected)
                {
                    await _hubConnection.StartAsync();
                }

                if (_currentSubscription is not null)
                {
                    await UnsubscribeAsync();
                }

                _currentSubscription = id;
                await _hubConnection.SendAsync("Subscribe", id);

                _hubConnection.On<Event>("DeviceEvent", BroadcastEvent);
            }
        }

        private async Task UnsubscribeAsync()
        {
            if (_hubConnection is not null && IsConnected && _currentSubscription is not null)
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

            foreach (var observer in _eventObservers.ToArray())
            {
                if (observer != null)
                {
                    observer.OnCompleted();
                }
            }
            _eventObservers.Clear();
        }

        private void BroadcastEvent(Event eventData)
        {
            if (eventData != null)
            {
                foreach (var observer in _eventObservers)
                {
                    observer.OnNext(eventData);
                }
            }
        }
    }
}
