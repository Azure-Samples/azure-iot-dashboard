using System.Collections.Concurrent;
using Azure.IoT.ModelsRepository;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Serialization;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Rest;
using Iot.PnpDashboard.Events;


namespace Iot.PnpDashboard.Devices
{
    public class DeviceService : IDeviceService, IAsyncDisposable
    {
        public IEnumerable<Device> OnlineDevices { get { return _onlineDevices.Values; } }

        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly RegistryManager _registryManager;
        private readonly DigitalTwinClient _digitalTwinClient;
        private readonly ModelsRepositoryClient _modelsRepositoryClient;
        private readonly ConcurrentDictionary<string, Device> _onlineDevices;

        public DeviceService(IConfiguration confiuration, ILogger<DeviceService> logger)
        {
            _configuration = confiuration;
            _logger = logger;
            _registryManager = RegistryManager.CreateFromConnectionString(_configuration.GetValue<string>("Azure:IotHub:ConnectionString"));
            _digitalTwinClient = DigitalTwinClient.CreateFromConnectionString(_configuration.GetValue<string>("Azure:IotHub:ConnectionString"));
            _modelsRepositoryClient = new ModelsRepositoryClient();
            _onlineDevices = new ConcurrentDictionary<string, Device>();
        }

        public void UpdateOnlineDevices(Event e)
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
