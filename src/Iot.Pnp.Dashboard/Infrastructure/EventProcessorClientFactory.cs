using Azure.Messaging.EventHubs;
using Iot.PnpDashboard.Configuration;

namespace Iot.PnpDashboard.Infrastructure
{
    public class EventProcessorClientFactory
    {
        private readonly AppConfiguration _configuration;
        private readonly CheckpointBlobClientFactory _blobClientFactory;
        //private readonly ILogger _logger;

        public EventProcessorClientFactory(
            AppConfiguration configuration,
            CheckpointBlobClientFactory blobClientFactory)
        {
            _configuration = configuration;
            _blobClientFactory = blobClientFactory;
        }

        public async Task<EventProcessorClient> CreateEventProcessorClientAsync()
        {
            EventHubConnectionOptions connectionOptions = new EventHubConnectionOptions()
            {
                ConnectionIdleTimeout = TimeSpan.FromSeconds(30)
            };

            EventHubsRetryOptions retryOptions = new EventHubsRetryOptions()
            {
                Mode = EventHubsRetryMode.Exponential,
                Delay = TimeSpan.FromSeconds(10),
                MaximumDelay = TimeSpan.FromSeconds(180),
                MaximumRetries = 9,
                TryTimeout = TimeSpan.FromSeconds(15)
            };

            EventProcessorClientOptions clientOptions = new EventProcessorClientOptions()
            {
                MaximumWaitTime = TimeSpan.FromSeconds(30),
                PrefetchCount = 250,
                CacheEventCount = 250,
                ConnectionOptions = connectionOptions,
                RetryOptions = retryOptions
            };

            if (_configuration.ManagedIdentityEnabled)
            {
                return await CreateEventProcessorClientWithMSIAsync(clientOptions);
            }

            return await CreateEventProcessorClientFromConnStrAsync(clientOptions);
        }

        private async Task<EventProcessorClient> CreateEventProcessorClientWithMSIAsync(EventProcessorClientOptions clientOptions)
        {
            throw new NotImplementedException();
        }

        private async Task<EventProcessorClient> CreateEventProcessorClientFromConnStrAsync(EventProcessorClientOptions clientOptions)
        {
            var iotHubConnectionString = _configuration.IotHubConnStr;
            var consumerGroup = _configuration.IotHubConsumerGroup;

            //TODO: Is possible to obtain the EH Built-in endpoint conn string in other way (REST API, Management API...)??
            var iotHubConnStrWithDeviceId = !iotHubConnectionString.Contains(";DeviceId=") ? $"{iotHubConnectionString};DeviceId=NoMatterWhichDeviceIs" : iotHubConnectionString;
            string ehConnString = EventHubConnectionResolver.GetConnectionString(iotHubConnStrWithDeviceId).Result;

            var checkpointStore = await _blobClientFactory.CreateContainerClientAsync();

            EventProcessorClient eventProcessor = new EventProcessorClient(checkpointStore, consumerGroup, ehConnString, clientOptions);

            return eventProcessor;
        }
    }
}