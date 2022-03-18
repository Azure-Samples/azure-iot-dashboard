using Azure.Messaging.EventHubs.Consumer;
using Iot.PnpDashboard.Extensions;
using System.Linq;

namespace Iot.PnpDashboard.Configuration
{
    public class AppConfiguration
    {
        private readonly IConfiguration _configuration;
        public string IotHubConnStr => _configuration.GetValueOrThrow<string>("Azure:IotHub:ConnectionString");
        public string? IotHubHostName => IotHubConnStr.Split(';').Where(x => x.Contains("HostName")).Select(s => s.Split('=').LastOrDefault()).FirstOrDefault();
        public string IotHubConsumerGroup => _configuration.GetValueOrDefault<string>("Azure:IotHub:ConsumerGroup", EventHubConsumerClient.DefaultConsumerGroupName);
        public string CheckpointStaConnString => _configuration.GetValueOrThrow<string>("Azure:CheckpointStorageAccount:ConnectionString");
        public string CheckpointStaContainer => _configuration.GetValueOrDefault<string>("Azure:CheckpointStorageAccount:Container", "iot-hub-checkpointing");
        public string? CheckpointStaAccountName => CheckpointStaConnString.Split(';').Where(x => x.Contains("AccountName")).Select(s => s.Split('=').LastOrDefault()).FirstOrDefault();
        public string SignalRConnStr => _configuration.GetValueOrThrow<string>("Azure:SignalR:ConnectionString");
        public string? SignalREndpoint => SignalRConnStr.Split(';').Where(x => x.Contains("Endpoint")).Select(s => s.Split('=').LastOrDefault()).FirstOrDefault();
        public string RedisConnStr => _configuration.GetValueOrThrow<string>("Azure:Redis:ConnectionString");
        public string? RedisHostName => RedisConnStr.Split(",").FirstOrDefault<string>()?.Split(":").FirstOrDefault();
        public string? RedisPort => RedisConnStr.Split(",").FirstOrDefault<string>()?.Split(":")[1];

        public AppConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
