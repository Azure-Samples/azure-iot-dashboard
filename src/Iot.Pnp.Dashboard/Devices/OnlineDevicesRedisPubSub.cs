using Iot.PnpDashboard.Configuration;
using Iot.PnpDashboard.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.IotHub.Models;
using Microsoft.Rest.TransientFaultHandling;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;

namespace Iot.PnpDashboard.Devices
{
    public class OnlineDevicesRedisPubSub : IOnlineDevices
    {
        private ConcurrentDictionary<string, Device> _onlineDevices;
        private readonly AppConfiguration _configuration;
        private readonly ConnectionMultiplexer _cacheConnection;
        private readonly ISubscriber _subscriber;

        public OnlineDevicesRedisPubSub(AppConfiguration configuration)
        {
            _onlineDevices = new ConcurrentDictionary<string, Device>();
            _configuration = configuration;
            _cacheConnection = ConnectionMultiplexer.Connect(_configuration.RedisConnStr);
            _subscriber = _cacheConnection.GetSubscriber();
            _subscriber.Subscribe("OnlineDevices", async (channel, message) =>
            {
                await UpdateAsync(JsonConvert.DeserializeObject<Event>(message));
            });
        }

        public async Task PublishAsync(Event e)
        {
            await _subscriber.PublishAsync("OnlineDevices", JsonConvert.SerializeObject(e), CommandFlags.FireAndForget);
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
            return Task.FromResult<IEnumerable<Device>>(
                _onlineDevices.Values
                .Where(d => !String.IsNullOrWhiteSpace(namePattern) ? d.DeviceId.StartsWith(namePattern) : true)
                .OrderBy(d=> d.DeviceId)
                .Skip(pageSize * pageOffset).Take(pageSize));
        }

        public Task<long> CountAsync() => Task.FromResult<long>(_onlineDevices.Count);

        public Task<IEnumerable<string>> GetIdsAsync(string? namePattern = null, int pageSize = 100, int pageOffset = 0)
        {
            return Task.FromResult<IEnumerable<string>>(
                _onlineDevices.Keys
                .Where(k => !String.IsNullOrWhiteSpace(namePattern) ? k.StartsWith(namePattern) : true)
                .OrderBy(k => k)
                .Skip(pageSize * pageOffset).Take(pageSize)
            );
        }

        public Task<Device?> GetDeviceAsync(string id)
        {
            return Task.FromResult<Device?>(_onlineDevices.ContainsKey(id) ? _onlineDevices[id] : null);
        }
    }
}
