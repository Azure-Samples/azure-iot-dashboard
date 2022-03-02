using IoT.Consumer.WebSite.Events;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using Azure.IoT.ModelsRepository;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Microsoft.Rest;
using Microsoft.Azure.Devices.Serialization;
using IoT.Consumer.WebSite.SignalR;

namespace IoT.Consumer.WebSite.Devices
{
    public class DeviceService : IDeviceService, IAsyncDisposable
    {
        public List<Device> Devices { get; } = new List<Device>();

        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly RegistryManager _registryManager;
        private readonly DigitalTwinClient _digitalTwinClient;
        private readonly ModelsRepositoryClient _modelsRepositoryClient;

        public DeviceService(IConfiguration confiuration, ILogger<DeviceService> logger)
        {
            _configuration = confiuration;
            _logger = logger;
            _registryManager = RegistryManager.CreateFromConnectionString(_configuration.GetValue<string>("Azure:IotHub:ConnectionString"));
            _digitalTwinClient = DigitalTwinClient.CreateFromConnectionString(_configuration.GetValue<string>("Azure:IotHub:ConnectionString"));
            _modelsRepositoryClient = new ModelsRepositoryClient();
        }

        public async Task<Twin?> GetDeviceTwinAsync(string? deviceId)
        {
            try
            {
                var twin = await _registryManager.GetTwinAsync(deviceId);
                return twin;
            }
            catch
            {
                //TODO: Manage properly
                //Unable to resolve twin for deviceId
                return null;
            }
        }

        public async Task<BasicDigitalTwin?> GetDigitalTwinAsync(string? deviceId)
        {
            try
            {
                HttpOperationResponse<BasicDigitalTwin, DigitalTwinGetHeaders> getDigitalTwinResponse = await _digitalTwinClient.GetDigitalTwinAsync<BasicDigitalTwin>(deviceId);
                var digitalTwin = getDigitalTwinResponse.Body;
                return digitalTwin;
            }
            catch
            {
                //TODO: Manage properly
                //Unable to resolve twin for deviceId
                return null;
            }
        }

        public async Task<ModelResult?> ResolveModelAsync(string? dtmi)
        {
            try
            {
                var modelResult = await _modelsRepositoryClient.GetModelAsync(dtmi, ModelDependencyResolution.Enabled);
                return modelResult;
            }
            catch
            {
                //TODO: Manage properly (if 404 not found in the default / defined repository)
                //Unable to resolve modelId.
                return null;
            }
        }

        public ValueTask DisposeAsync()
        {
            _registryManager.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
