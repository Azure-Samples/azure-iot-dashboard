using Iot.PnpDashboard.Configuration;
using Iot.PnpDashboard.Events;
using Microsoft.Azure.Management.IotHub.Models;
using Microsoft.Rest.TransientFaultHandling;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;

namespace Iot.PnpDashboard.Devices
{

    public class OnlineDevicesCache : IOnlineDevices
    {
        private readonly Dictionary<string, Device> _onlineDevices;
        private readonly AppConfiguration _configuration;
        private readonly ConnectionMultiplexer _cacheConnection;
        private IDatabaseAsync _onlineDevicesCache;

        public OnlineDevicesCache(AppConfiguration configuration)
        {
            _configuration = configuration;

            _cacheConnection = ConnectionMultiplexer.Connect(_configuration.RedisConnStr); 

            _onlineDevicesCache = _cacheConnection.GetDatabase();
            _onlineDevices = new Dictionary<string, Device>();
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
                        device.MessageSource = e.MessageSource;
                        device.LastOperation = e.Operation;
                        device.LastOperationTimestamp = e.EnqueuedTime;
                        device.Disconnected = e.MessageSource == "deviceConnectionStateEvents" && e.Operation == "deviceDisconnected";
                    }

                    if (e.MessageSource == "Telemetry")
                    {
                        device.LastTelemetryTimestamp = e.EnqueuedTime;
                        device.Disconnected = false;
                    }
                }
                else
                {
                    device = new Device()
                    {
                        DeviceId = e.DeviceId,
                        ModelId = e.ModelId ?? string.Empty,
                        MessageSource = e.Operation is not null ? e.MessageSource : null,
                        LastOperation = e.Operation is not null ? e.Operation : null,
                        LastTelemetryTimestamp = e.MessageSource == "Telemetry" ? e.EnqueuedTime : null,
                        LastOperationTimestamp = e.Operation is not null ? e.EnqueuedTime : null,
                        Disconnected = e.MessageSource == "deviceConnectionStateEvents" && e.Operation == "deviceDisconnected"
                    };
                }
                await _onlineDevicesCache.SetAddAsync("OnlineDevices", e.DeviceId);
                await _onlineDevicesCache.StringSetAsync(device.DeviceId, JsonConvert.SerializeObject(device), TimeSpan.FromMinutes(30), When.Always, CommandFlags.FireAndForget);

            }
        }

        public async Task<long> CountAsync() => await _onlineDevicesCache.SetLengthAsync("OnlineDevices");
   
        public async Task<IEnumerable<Device>> GetAsync(string? namePattern=default, int pageSize = 100, int page = 0)
        {
            try
            {
                List<Device> devices = new List<Device>();

                var currentDevices = await _onlineDevicesCache.SetMembersAsync("OnlineDevices");
                if (currentDevices != null)
                {
                    var pagedDevices = currentDevices
                    .Where(v => !String.IsNullOrWhiteSpace(namePattern) ? v.ToString().StartsWith(namePattern) : true)
                    .OrderBy(v => v)
                    .Skip(pageSize * page).Take(pageSize);
                
                    foreach (var deviceId in pagedDevices)
                    {
                        var deviceData = await _onlineDevicesCache.StringGetAsync(deviceId.ToString()); //Can be evicted due to expiration
                        if (deviceData.HasValue)
                        {
                            devices.Add(JsonConvert.DeserializeObject<Device>(deviceData));
                        }
                        else
                        {
                            bool result = await _onlineDevicesCache.SetRemoveAsync("OnlineDevices", deviceId);
                        }
                    }
                }
                return devices;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
