using Azure.IoT.ModelsRepository;
using Microsoft.Azure.Devices.Serialization;
using Microsoft.Azure.Devices.Shared;
using Iot.PnpDashboard.Events;

namespace Iot.PnpDashboard.Devices
{
    public interface IDeviceService : IAsyncDisposable
    {
        Task<IEnumerable<Device>> GetOnlineDevicesAsync(string? namePattern = default, int pageSize = 100, int pageOffset = 0);
        Task<long> OnlineDevicesCountAsync();
        Task<Twin?> GetDeviceTwinAsync(string? deviceId);
        Task<BasicDigitalTwin?> GetDigitalTwinAsync(string? deviceId);
        Task<ModelResult?> ResolveModelAsync(string? modelId); 
    }
}
