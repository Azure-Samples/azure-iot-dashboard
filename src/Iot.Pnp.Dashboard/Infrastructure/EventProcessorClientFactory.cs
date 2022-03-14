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

            //if (_configuration.ManagedIdentityEnabled)
            //{
            //    var fullyQualifiedNamespace = ehConnString.Split(';').Where(s=> s.StartsWith("Endpoint")).Select(s=> s.Replace("Endpoint=sb://", "").TrimEnd('/')).FirstOrDefault();
            //    var eventHubName = ehConnString.Split(';').Where(s => s.StartsWith("EntityPath")).Select(s => s.Replace("EntityPath=", "")).FirstOrDefault();

            //    return CreateEventProcessorClientWithMSI(checkpointStore, fullyQualifiedNamespace, eventHubName, clientOptions);
            //}
            //else
            //{
            //    return CreateEventProcessorClientFromConnStr(checkpointStore, ehConnString, clientOptions);
            //}
            return CreateEventProcessorClientFromConnStr(checkpointStore, ehConnString, clientOptions);
        }


        private EventProcessorClient CreateEventProcessorClientWithMSI(BlobContainerClient checkpointStore, string? fullyQualifiedNamespace, string? eventHubName, EventProcessorClientOptions clientOptions)
        {
            //TODO: NOTE ABOUT Managed Identity with IoT HUB:
            //  TO BE ABLE TO RETREIVE THE EVENT HUB CONNECTION STRING DYNAMICALLY, IT IS REQUIRED THE IOT HUB CONNECTION STRING
            //  WE CAN OPT TO USE IoT Hub Conn Str or the EH Built-in Endpoint. The latter option will limit the ability to be resilent in case of failover.
            
            DefaultAzureCredentialOptions options = new DefaultAzureCredentialOptions()
            {
                ManagedIdentityClientId = _configuration.ManagedIdentityClientId ?? null
            };
            var credential = new DefaultAzureCredential(options);

            EventProcessorClient eventProcessor = new EventProcessorClient(checkpointStore, _configuration.IotHubConsumerGroup, fullyQualifiedNamespace, eventHubName, credential, clientOptions);

            return eventProcessor;
        }

        private EventProcessorClient CreateEventProcessorClientFromConnStr(BlobContainerClient checkpointStore, string ehConnString, EventProcessorClientOptions clientOptions)
        {
            //TODO: NOTE ABOUT Managed Identity with IoT HUB:
            //  TO BE ABLE TO RETREIVE THE EVENT HUB CONNECTION STRING DYNAMICALLY, IT IS REQUIRED THE IOT HUB CONNECTION STRING
            //  WE CAN OPT TO USE IoT Hub Conn Str or the EH Built-in Endpoint. The latter option will limit the ability to be resilent in case of failover.

            EventProcessorClient eventProcessor = new EventProcessorClient(checkpointStore, _configuration.IotHubConsumerGroup, ehConnString, clientOptions);

            return eventProcessor;
        }
    }
}