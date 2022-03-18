using Iot.PnpDashboard.Configuration;
using Iot.PnpDashboard.EventBroadcast;
using Iot.PnpDashboard.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.Management.IotHub.Models;
using Microsoft.Rest.TransientFaultHandling;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace Iot.PnpDashboard.Devices
{
    public class OnlineDevicesSignalR : IAsyncDisposable
    {
        private Dictionary<string, Device>_onlineDevices;
        private HubConnection? _hubConnection;

        public OnlineDevicesSignalR(string baseUrl)
        {
            _onlineDevices = new Dictionary<string, Device>();
            SetupHubConnection(baseUrl);
        }

        private void SetupHubConnection(string baseUrl)
        {
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

        public async Task SubscribeOnlineDevicesAsync()
        {
            if (_hubConnection is not null)
            {
                if (_hubConnection.State != HubConnectionState.Connected)
                {
                    await _hubConnection.StartAsync();
                    _hubConnection.On<Event>("DeviceEvent", UpdateAsync);
                }

                await _hubConnection.SendAsync("Subscribe", "OnlineDevices");
            }
        }

        public async Task UpdateAsync(Event e)
        {
            if (e is not null)
            {
                if (_onlineDevices.ContainsKey(e.DeviceId))
                {
                    Device toUpdate = _onlineDevices[e.DeviceId];
                    if (e.ModelId != null)
                    {
                        toUpdate.ModelId = e.ModelId;
                    }

                    if (e.Operation != null)
                    {
                        toUpdate.OperationSource = e.MessageSource;
                        toUpdate.LastOperation = e.Operation;
                        toUpdate.OperationTimestamp = e.EnqueuedTime;
                        toUpdate.Disconnected = e.MessageSource == "deviceConnectionStateEvents" && e.Operation == "deviceDisconnected";
                    }

                    if (e.MessageSource == "Telemetry")
                    {
                        toUpdate.TelemetryTimestamp = e.EnqueuedTime;
                        toUpdate.Disconnected = false;
                    }
                    _onlineDevices[e.DeviceId] = toUpdate;
                }
                else
                {
                    var device = new Device()
                    {
                        DeviceId = e.DeviceId,
                        ModelId = e.ModelId ?? string.Empty,
                        OperationSource = e.Operation is not null ? e.MessageSource : null,
                        LastOperation = e.Operation is not null ? e.Operation : null,
                        TelemetryTimestamp = e.MessageSource == "Telemetry" ? e.EnqueuedTime : null,
                        OperationTimestamp = e.Operation is not null ? e.EnqueuedTime : null,
                        Disconnected = e.MessageSource == "deviceConnectionStateEvents" && e.Operation == "deviceDisconnected"
                    };
                    _onlineDevices.TryAdd(e.DeviceId, device);
                }
            }
            await Task.FromResult(new OkResult());
        }

        public Task<IEnumerable<Device>> GetAsync(string? namePattern = default, int pageSize = 100, int pageOffset = 0)
        {
            return Task.FromResult<IEnumerable<Device>>(_onlineDevices.Values);
        }

        public Task<long> CountAsync() => Task.FromResult<long>(_onlineDevices.Count);

        public Task<IEnumerable<string>> GetIdsAsync(string? namePattern = null, int pageSize = 100, int page = 0)
        {
            return Task.FromResult<IEnumerable<string>>(_onlineDevices.Keys);
        }

        public Task<Device?> GetDeviceAsync(string id)
        {
            return Task.FromResult<Device?>(_onlineDevices.ContainsKey(id) ? _onlineDevices[id] : null);
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.SendAsync("Subscribe", "OnlineDevices");
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
