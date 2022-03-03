using Azure.IoT.ModelsRepository;
using IoT.Consumer.WebSite.Events;
using Microsoft.Azure.Devices.Serialization;
using Microsoft.Azure.Devices.Shared;

namespace IoT.Consumer.WebSite.Devices
{
    public interface IDeviceService : IAsyncDisposable
    {
        IEnumerable<Device> OnlineDevices { get; }
        void UpdateOnlineDevices(Event e);

        Task<Twin?> GetDeviceTwinAsync(string? deviceId);
        Task<BasicDigitalTwin?> GetDigitalTwinAsync(string? deviceId);
        Task<ModelResult?> ResolveModelAsync(string? modelId); 
    }
}
