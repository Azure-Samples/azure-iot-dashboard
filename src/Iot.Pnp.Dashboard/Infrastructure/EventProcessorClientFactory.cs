using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Storage.Blobs;
using Iot.PnpDashboard.Configuration;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Management.IotHub;
using Microsoft.Rest;
using System.Globalization;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Iot.PnpDashboard.Infrastructure
{
    public class EventProcessorClientFactory
    {
        private readonly AppConfiguration _configuration;
        private readonly CheckpointBlobClientFactory _blobClientFactory;

        public EventProcessorClientFactory(
            AppConfiguration configuration)
        {
            _configuration = configuration;
            _blobClientFactory = new CheckpointBlobClientFactory(_configuration);
        }

        public async Task<EventProcessorClient> CreateAsync()
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

            string ehConnString = EventHubConnectionResolver.GetConnectionString(_configuration.IotHubConnStr).Result;
            
            var checkpointStore = await _blobClientFactory.CreateAsync();

            return CreateEventProcessorClientFromConnStr(checkpointStore, ehConnString, clientOptions);
        }

        private EventProcessorClient CreateEventProcessorClientFromConnStr(BlobContainerClient checkpointStore, string ehConnString, EventProcessorClientOptions clientOptions)
        {
            EventProcessorClient eventProcessor = new EventProcessorClient(checkpointStore, _configuration.IotHubConsumerGroup, ehConnString, clientOptions);

            return eventProcessor;
        }
    }
}