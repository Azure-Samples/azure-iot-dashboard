using Iot.PnpDashboard.Configuration;
using Iot.PnpDashboard.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Management.IotHub.Models;
using Microsoft.Rest.TransientFaultHandling;
using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.Maintenance;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;

namespace Iot.PnpDashboard.Devices
{

    public class OnlineDevicesCache : IOnlineDevices
    {
        private readonly Timer? _timer;
        private readonly AppConfiguration _configuration;
        private readonly ConnectionMultiplexer _cacheConnection;
        private readonly IDatabaseAsync _onlineDevicesCache;
        private readonly Dictionary<string, Event> _eventsBatch;
        private readonly Timer _batchTimer;
        private readonly Object _lockObject = new Object();


        public OnlineDevicesCache(AppConfiguration configuration)
        {
            _configuration = configuration;
            _cacheConnection = ConnectionMultiplexer.Connect(_configuration.RedisConnStr);
            _onlineDevicesCache = _cacheConnection.GetDatabase();
            _eventsBatch = new Dictionary<string, Event>();
            _timer = SetupCleanUpTimer();
            _batchTimer = new Timer(async (object? stateInfo) => await UpdateCache(null), new AutoResetEvent(false), 0, 5000);
        }

        public async Task UpdateAsync(Event e)
        {
            if (e is not null)
            {
                var deviceInfo = await _onlineDevicesCache.StringGetAsync(e.DeviceId);
                Device? device = (deviceInfo.HasValue) ? JsonConvert.DeserializeObject<Device>(deviceInfo) : null;

                if (device != null)
                {
                    if (e.ModelId != null)
                    {
                        device.ModelId = e.ModelId;
                    }

                    if (e.Operation != null)
                    {
                        device.OperationSource = e.MessageSource;
                        device.LastOperation = e.Operation;
                        device.OperationTimestamp = e.EnqueuedTime;
                        device.Disconnected = e.MessageSource == "deviceConnectionStateEvents" && e.Operation == "deviceDisconnected";
                    }

                    if (e.MessageSource == "Telemetry")
                    {
                        device.TelemetryTimestamp = e.EnqueuedTime;
                        device.Disconnected = false;
                    }
                }
                else
                {
                    device = new Device()
                    {
                        DeviceId = e.DeviceId,
                        ModelId = e.ModelId ?? string.Empty,
                        OperationSource = e.Operation is not null ? e.MessageSource : null,
                        LastOperation = e.Operation is not null ? e.Operation : null,
                        TelemetryTimestamp = e.MessageSource == "Telemetry" ? e.EnqueuedTime : null,
                        OperationTimestamp = e.Operation is not null ? e.EnqueuedTime : null,
                        Disconnected = e.MessageSource == "deviceConnectionStateEvents" && e.Operation == "deviceDisconnected"
                    };
                }
                if (!_eventsBatch.ContainsKey(e.DeviceId))
                {
                    _eventsBatch.Add(e.DeviceId, e);
                }
                else
                {
                    _eventsBatch[e.DeviceId] = e;
                }
            }
        }

        private async Task UpdateCache(object? stateInfo)
        {
            //TODO: Mucho timer!
            try
            {
                Dictionary<string, Event> localBatch = new Dictionary<string, Event>(_eventsBatch);
                _eventsBatch.Clear();

                Parallel.ForEach(localBatch, async item =>
                {
                    //Add the device to the OnlineDevicesIds set
                    await _onlineDevicesCache.SetAddAsync("OnlineDevicesIds", item.Key, CommandFlags.FireAndForget);

                    //If in 30 minutes we didn't have information from this device, we remove it from the "online devices"
                    await _onlineDevicesCache.StringSetAsync(item.Key, JsonConvert.SerializeObject(item.Value), TimeSpan.FromMinutes(30), When.Always, CommandFlags.FireAndForget);
                });

                //foreach (var key in localBatch.Keys)
                //{
                //    //Add the device to the OnlineDevicesIds set
                //    await _onlineDevicesCache.SetAddAsync("OnlineDevicesIds", key, CommandFlags.FireAndForget);

                //    //If in 30 minutes we didn't have information from this device, we remove it from the "online devices"
                //    await _onlineDevicesCache.StringSetAsync(key, JsonConvert.SerializeObject(localBatch[key]), TimeSpan.FromMinutes(30), When.Always, CommandFlags.FireAndForget);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //TODO: Log the exception; we silent just in case
            }


        }

        public async Task<long> CountAsync() => await _onlineDevicesCache.SetLengthAsync("OnlineDevicesIds");

        public async Task<Device?> GetDeviceAsync(string deviceId)
        {
            var deviceData = await _onlineDevicesCache.StringGetAsync(deviceId.ToString()); //Can be evicted due to expiration
            if (deviceData.HasValue)
            {
                return JsonConvert.DeserializeObject<Device>(deviceData);
            }
            else
            {
                bool result = await _onlineDevicesCache.SetRemoveAsync("OnlineDevicesIds", deviceId);
                return null;
            }
        }
        public async Task<IEnumerable<string>> GetIdsAsync(string? namePattern = default, int pageSize = 100, int page = 0)
        {
            try
            {

                var currentDevices = await _onlineDevicesCache.SetMembersAsync("OnlineDevicesIds");
                if (currentDevices != null)
                {
                    var pagedDevices = currentDevices
                    .Select(v => v.ToString())
                    .Where(id => !String.IsNullOrWhiteSpace(namePattern) ? id.StartsWith(namePattern) : true)
                    .OrderBy(s => s)
                    .Skip(pageSize * page).Take(pageSize);
                    return pagedDevices;
                }
                else
                {
                    return new List<string>();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IEnumerable<Device>> GetAsync(string? namePattern = default, int pageSize = 100, int page = 0)
        {
            try
            {
                List<Device> devices = new List<Device>();

                var ids = await GetIdsAsync(namePattern, pageSize, page);

                foreach (var id in ids)
                {
                    Device? device = await GetDeviceAsync(id);
                    if (device is not null)
                    {
                        devices.Add(device);
                    }
                }
                return devices;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Timer SetupCleanUpTimer()
        {
            int cleanUpPeriod = 60000;
            Timer timer = new Timer(async (object? stateinfo) =>
            {
                try
                {
                    await CleanUpStaleDevicesIds();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    //TODO: Log the exception; we silent just in case
                }
            }, new AutoResetEvent(false), 0, cleanUpPeriod);
            return timer;
        }

        private async Task CleanUpStaleDevicesIds()
        {
            //TODO: We can take advantage of using Redis in standar tier and use pub/sub to subscribe to device data expirations
            //instead of having a background thread cleaning stale devices
            var token = Guid.NewGuid();
            bool? cleaner = await _onlineDevicesCache.LockTakeAsync("Cleaner", token.ToString(), TimeSpan.FromMinutes(5));
            if (cleaner.HasValue && cleaner.Value)
            {
                try
                {
                    var currentDevicesIds = await _onlineDevicesCache.SetMembersAsync("OnlineDevicesIds");
                    if (currentDevicesIds != null)
                    {
                        foreach (var deviceId in currentDevicesIds)
                        {
                            var deviceData = await _onlineDevicesCache.StringGetAsync(deviceId.ToString());
                            if (!deviceData.HasValue)
                            {
                                await _onlineDevicesCache.SetRemoveAsync("OnlineDevicesIds", deviceId);
                            }
                        }
                    }
                }
                finally
                {
                    await _onlineDevicesCache.LockReleaseAsync("Cleaner", token.ToString());
                }
            }
        }
    }
}
