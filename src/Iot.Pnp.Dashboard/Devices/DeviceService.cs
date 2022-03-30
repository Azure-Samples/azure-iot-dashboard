using System.Collections.Concurrent;
using Azure.IoT.ModelsRepository;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Serialization;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Rest;
using Iot.PnpDashboard.Events;
using Iot.PnpDashboard.Infrastructure;
using Iot.PnpDashboard.Configuration;

namespace Iot.PnpDashboard.Devices
{
    public class DeviceService : IDeviceService, IAsyncDisposable
    {
        private readonly AppConfiguration _configuration;
        private readonly OnlineDevicesService _onlineDevices;
        private readonly ILogger _logger;
        private readonly RegistryManager _registryManager;
        private readonly DigitalTwinClient _digitalTwinClient;
        private readonly ModelsRepositoryClient _modelsRepositoryClient;

        public DeviceService(AppConfiguration configuration, OnlineDevicesService onlineDevices, ILogger<DeviceService> logger)
        {
            
            _configuration = configuration;
            _onlineDevices = onlineDevices;
            _logger = logger;
            
            _modelsRepositoryClient = new ModelsRepositoryClient();
            
            var registryManagerFactory = new RegistryManagerFactory(_configuration);
            _registryManager = registryManagerFactory.Create();

            var digitalTwinClientFactory = new DigitalTwinClientFactory(_configuration);
            _digitalTwinClient = digitalTwinClientFactory.Create();
        }

        public OnlineDevicesService OnlineDevices => _onlineDevices;

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
