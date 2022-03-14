using Iot.PnpDashboard.Configuration;
using Iot.PnpDashboard.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.IotHub.Models;
using Microsoft.Rest.TransientFaultHandling;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Reflection.Metadata.Ecma335;

namespace Iot.PnpDashboard.Devices
{
    public class OnlineDevicesList : IOnlineDevices
    {
        private Dictionary<string, Device>_onlineDevices;

        public OnlineDevicesList()
        {
            _onlineDevices = new Dictionary<string, Device>();
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
                        toUpdate.MessageSource = e.MessageSource;
                        toUpdate.LastOperation = e.Operation;
                        toUpdate.LastOperationTimestamp = e.EnqueuedTime;
                        toUpdate.Disconnected = e.MessageSource == "deviceConnectionStateEvents" && e.Operation == "deviceDisconnected";
                    }

                    if (e.MessageSource == "Telemetry")
                    {
                        toUpdate.LastTelemetryTimestamp = e.EnqueuedTime;
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
                        MessageSource = e.Operation is not null ? e.MessageSource : null,
                        LastOperation = e.Operation is not null ? e.Operation : null,
                        LastTelemetryTimestamp = e.MessageSource == "Telemetry" ? e.EnqueuedTime : null,
                        LastOperationTimestamp = e.Operation is not null ? e.EnqueuedTime : null,
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


    }
}
