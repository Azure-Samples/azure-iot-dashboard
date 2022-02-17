using Azure.IoT.ModelsRepository;
using Microsoft.Azure.Devices.Serialization;
using Microsoft.Azure.Devices.Shared;

namespace IoT.Consumer.WebSite.Devices
{
    public interface IDeviceService : IAsyncDisposable
    {
        List<Device> Devices { get; }

        Task<Twin?> GetDeviceTwinAsync(string? deviceId);
        Task<BasicDigitalTwin?> GetDigitalTwinAsync(string? deviceId);
        Task<ModelResult?> ResolveModelAsync(string? modelId); 
    }
}
