using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Iot.PnpDashboard.Events;

namespace Iot.PnpDashboard.EventBroadcast
{
    public class IotEventsHubClient : IObservable<Event>, IAsyncDisposable
    {
        //TODO: Message Pack?
        private readonly ILogger _logger;
        private HubConnection? _hubConnection;
        private List<IObserver<Event>> _eventObservers = new List<IObserver<Event>>();
        private readonly List<string> _currentSubscriptions;

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public IotEventsHubClient(ILogger<IotEventsHubClient> logger, string baseUrl)
        {
            _logger = logger;
            _currentSubscriptions = new List<string>();
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
            {
                _eventObservers.Add(observer);
            }

            return new Unsubscriber<Event>(_eventObservers, observer);
        }

        public async Task SubscribeTelemetryAsync()
        {
            await SubscribeTelemetryAsync(new string[] { "all-devices" });
        }
        public async Task SubscribeTelemetryAsync(string[] ids)
        {
            if (_hubConnection is not null)
            {
                if (!IsConnected)
                {
                    await _hubConnection.StartAsync();
                    _hubConnection.On<Event>("DeviceEvent", BroadcastEvent);
                }

                if (_currentSubscriptions.Count > 0)
                {
                    await UnsubscribeAsync();
                }

                _currentSubscriptions.AddRange(ids);
                foreach (var id in _currentSubscriptions)
                {
                    await _hubConnection.SendAsync("Subscribe", id);
                }
            }
        }

        private async Task UnsubscribeAsync()
        {
            if (_hubConnection is not null && IsConnected && _currentSubscriptions.Count > 0)
            {
                foreach (var id in _currentSubscriptions)
                {
                    await _hubConnection.SendAsync("Unsubscribe", id);
                }
                _currentSubscriptions.Clear();
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
