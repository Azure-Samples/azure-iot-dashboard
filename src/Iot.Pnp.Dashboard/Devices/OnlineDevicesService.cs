using Azure.Identity;
using Iot.PnpDashboard.Configuration;
using Iot.PnpDashboard.EventBroadcast;
using Iot.PnpDashboard.Events;
using Iot.PnpDashboard.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.IotHub.Models;
using Microsoft.Rest.TransientFaultHandling;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;

namespace Iot.PnpDashboard.Devices
{
    public class OnlineDevicesService
    {
        private readonly ConnectionMultiplexer _cacheConnection;
        private readonly Timer _cleanupTimer;
        private ISubscriber? _subscriber;

        private ConcurrentDictionary<string, Device> _onlineDevices;
        private ImmutableDictionary<string, Device> _stopSendingDevices;
        private ImmutableDictionary<string, Device> _disconnectedDevices;

        private const int CleanUpThresholdMinutes = 30;

        public const string OnlineDevicesUpdatesChannel = "OnlineDevices";

        public OnlineDevicesService(RedisConnectionFactory redisConnectionFactory)
        {
            _onlineDevices = new ConcurrentDictionary<string, Device>();
            _stopSendingDevices = ImmutableDictionary<string, Device>.Empty;
            _disconnectedDevices = ImmutableDictionary<string, Device>.Empty;
            _cacheConnection = redisConnectionFactory.GetConnection();
            _cleanupTimer = SetupCleanupTimer();
            SubscribeOnlineDevices();
        }

        private void SubscribeOnlineDevices()
        {
            _subscriber = _cacheConnection.GetSubscriber();
            _subscriber.Subscribe("OnlineDevices", async (channel, message) => await Task.Factory.StartNew(() => UpdateOnlineDevices(channel, message)));
        }
        private Timer SetupCleanupTimer()
        {
            return new Timer((state) =>
            {
                CleanUpDevices();
            }, new AutoResetEvent(false), 60000, CleanUpThresholdMinutes*60*1000);
        }

        public List<Device> Get(string? namePattern = default, int pageSize = 100, int pageOffset = 0)
        {
            return _onlineDevices.Values
                .Where(d => !String.IsNullOrWhiteSpace(namePattern) ? d.DeviceId.StartsWith(namePattern) : true)
                .OrderBy(d => d.DeviceId)
                .Skip(pageSize * pageOffset).Take(pageSize).ToList();
        }

        public long Count => _onlineDevices.Count;

        public List<string> GetIds(string? namePattern = null, int pageSize = 100, int pageOffset = 0)
        {
            return _onlineDevices.Keys
                .Where(k => !String.IsNullOrWhiteSpace(namePattern) ? k.StartsWith(namePattern) : true)
                .OrderBy(k => k)
                .Skip(pageSize * pageOffset).Take(pageSize).ToList();
        }

        public Device? GetDevice(string id)
        {
            Device? device;
            _onlineDevices.TryGetValue(id, out device);
            return device;
        }

        private void CleanUpDevices()
        {
            //TODO: Review for chances on make it more performant
            //All devices not sending data 30 mins ago, are moved to "disconected" and removed from "online"

            var _prevOnlineDevices = Interlocked.Exchange(ref _onlineDevices, new ConcurrentDictionary<string, Device>(
                _onlineDevices.Where(d => d.Value.TelemetryTimestamp > DateTime.UtcNow.AddMinutes(-CleanUpThresholdMinutes))));

            Interlocked.Exchange(ref _stopSendingDevices,
                _prevOnlineDevices.Where(d => d.Value.TelemetryTimestamp <= DateTime.UtcNow.AddMinutes(-CleanUpThresholdMinutes)).ToImmutableDictionary<string, Device>());
        }

        private void UpdateOnlineDevices(RedisChannel channel, RedisValue message)
        {
            var iotEvent = JsonConvert.DeserializeObject<Event>(message);
            if (iotEvent is not null)
            {
                if (iotEvent.MessageSource == "deviceConnectionStateEvents" && iotEvent.Operation == "deviceDisconnected")
                {
                    Device? disconnectedDevice;
                    if (_onlineDevices.TryRemove(iotEvent.DeviceId, out disconnectedDevice))
                    {
                        Interlocked.Exchange(ref _disconnectedDevices, _disconnectedDevices.Add(disconnectedDevice.DeviceId, disconnectedDevice));
                    }
                }
                else
                {
                    _onlineDevices.AddOrUpdate(
                    key: iotEvent.DeviceId,
                    addValue: new Device()
                    {
                        DeviceId = iotEvent.DeviceId,
                        ModelId = iotEvent.ModelId ?? string.Empty,
                        OperationSource = iotEvent.Operation is not null ? iotEvent.MessageSource : null,
                        LastOperation = iotEvent.Operation is not null ? iotEvent.Operation : null,
                        TelemetryTimestamp = iotEvent.MessageSource == "Telemetry" ? iotEvent.EnqueuedTime : null,
                        TelemetryProcessorOffset = iotEvent.TelemetryProcessorOffset,
                        TelemetryBroadcastOffset = DateTimeOffset.UtcNow,
                        OperationTimestamp = iotEvent.Operation is not null ? iotEvent.EnqueuedTime : null,
                        Disconnected = iotEvent.MessageSource == "deviceConnectionStateEvents" && iotEvent.Operation == "deviceDisconnected"
                    },
                    updateValueFactory: (id, toUpdate) =>
                    {
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
                            toUpdate.TelemetryProcessorOffset = iotEvent.TelemetryProcessorOffset;
                            toUpdate.TelemetryBroadcastOffset = DateTimeOffset.UtcNow;
                            toUpdate.Disconnected = false;
                        }

                        return toUpdate;
                    });
                }
            }
        }
    }
}
