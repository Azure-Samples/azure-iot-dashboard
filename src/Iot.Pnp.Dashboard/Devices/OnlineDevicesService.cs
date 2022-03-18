using Iot.PnpDashboard.Configuration;
using Iot.PnpDashboard.Events;
using Iot.PnpDashboard.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.IotHub.Models;
using Microsoft.Rest.TransientFaultHandling;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;

namespace Iot.PnpDashboard.Devices
{
    public class OnlineDevicesService
    {
        private ConcurrentDictionary<string, Device> _onlineDevices;
        private readonly ConnectionMultiplexer _cacheConnection;
        private readonly ISubscriber _subscriber;

        public const string Channel = "OnlineDevices";

        public OnlineDevicesService(RedisConnectionFactory redisConnectionFactory)
        {
            _onlineDevices = new ConcurrentDictionary<string, Device>();
            _cacheConnection = redisConnectionFactory.GetConnection();
            _subscriber = _cacheConnection.GetSubscriber();
            _subscriber.Subscribe("OnlineDevices", async (channel, message) => await UpdateAsync(channel, message).ConfigureAwait(false));
        }

        public async Task UpdateAsync(RedisChannel channel, RedisValue message)
        {
            var iotEvent = JsonConvert.DeserializeObject<Event>(message);
            if (iotEvent is not null)
            {
                if (_onlineDevices.ContainsKey(iotEvent.DeviceId))
                {
                    Device toUpdate = _onlineDevices[iotEvent.DeviceId];
                    if (iotEvent.ModelId != null)
                    {
                        toUpdate.ModelId = iotEvent.ModelId;
                    }

                    if (iotEvent.Operation != null)
                    {
                        toUpdate.OperationSource = iotEvent.MessageSource;
                        toUpdate.LastOperation = iotEvent.Operation;
                        toUpdate.OperationTimestamp = iotEvent.EnqueuedTime;
                        toUpdate.Disconnected = iotEvent.MessageSource == "deviceConnectionStateEvents" && iotEvent.Operation == "deviceDisconnected";
                    }

                    if (iotEvent.MessageSource == "Telemetry")
                    {
                        toUpdate.TelemetryTimestamp = iotEvent.EnqueuedTime;
                        toUpdate.Disconnected = false;
                    }
                    _onlineDevices[iotEvent.DeviceId] = toUpdate;
                }
                else
                {
                    var device = new Device()
                    {
                        DeviceId = iotEvent.DeviceId,
                        ModelId = iotEvent.ModelId ?? string.Empty,
                        OperationSource = iotEvent.Operation is not null ? iotEvent.MessageSource : null,
                        LastOperation = iotEvent.Operation is not null ? iotEvent.Operation : null,
                        TelemetryTimestamp = iotEvent.MessageSource == "Telemetry" ? iotEvent.EnqueuedTime : null,
                        OperationTimestamp = iotEvent.Operation is not null ? iotEvent.EnqueuedTime : null,
                        Disconnected = iotEvent.MessageSource == "deviceConnectionStateEvents" && iotEvent.Operation == "deviceDisconnected"
                    };
                    _onlineDevices.TryAdd(iotEvent.DeviceId, device);
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
