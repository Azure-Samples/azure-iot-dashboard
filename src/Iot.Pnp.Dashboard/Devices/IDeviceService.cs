using Azure.IoT.ModelsRepository;
using Microsoft.Azure.Devices.Serialization;
using Microsoft.Azure.Devices.Shared;
using Iot.PnpDashboard.Events;

namespace Iot.PnpDashboard.Devices
{
    public interface IDeviceService : IAsyncDisposable
    {
        OnlineDevicesService OnlineDevices { get; }
        Task<Twin?> GetDeviceTwinAsync(string? deviceId);
        Task<BasicDigitalTwin?> GetDigitalTwinAsync(string? deviceId);
        Task<ModelResult?> ResolveModelAsync(string? modelId); 
    }
}
