using Azure.Messaging.EventHubs.Consumer;
using Iot.PnpDashboard.Extensions;
using System.Linq;

namespace Iot.PnpDashboard.Configuration
{
    public class AppConfiguration
    {
        private readonly IConfiguration _configuration;

        public bool ManagedIdentityEnabled => _configuration.GetValueOrDefault<bool>("Azure:ManagedIdentity:Enabled", false);
        public string? ManagedIdentityClientId => _configuration.GetValueOrDefault<string>("Azure:ManagedIdentity:ClientId", default);
        public string IotHubConnStr => _configuration.GetValueOrThrow<string>("Azure:IotHub:ConnectionString");
        public string? IotHubHostName => IotHubConnStr.Split(';').Where(x => x.Contains("HostName")).Select(s => s.Split('=').LastOrDefault()).FirstOrDefault();
        public string IotHubConsumerGroup => _configuration.GetValueOrDefault<string>("Azure:IotHub:ConsumerGroup", EventHubConsumerClient.DefaultConsumerGroupName);
        public string CheckpointStaConnString => !ManagedIdentityEnabled ? _configuration.GetValueOrThrow<string>("Azure:CheckpointStorageAccount:ConnectionString") : string.Empty;
        public string CheckpointStaContainer => _configuration.GetValueOrDefault<string>("Azure:CheckpointStorageAccount:Container", "iot-hub-checkpointing");
        public string? CheckpointStaAccountName => ManagedIdentityEnabled ? _configuration.GetValueOrThrow<string>("Azure:CheckpointStorageAccount:AccountName") : CheckpointStaConnString.Split(';').Where(x => x.Contains("AccountName")).Select(s => s.Split('=').LastOrDefault()).FirstOrDefault();
        public string SignalRConnStr => !ManagedIdentityEnabled ? _configuration.GetValueOrThrow<string>("Azure:SignalR:ConnectionString") : string.Empty;
        public string? SignalREndpoint => ManagedIdentityEnabled ? _configuration.GetValueOrThrow<string>("Azure:SignalR:HostName") : SignalRConnStr.Split(';').Where(x => x.Contains("Endpoint")).Select(s => s.Split('=').LastOrDefault()).FirstOrDefault();
        public string RedisConnStr => _configuration.GetValueOrThrow<string>("Azure:Redis:ConnectionString");
        public string? RedisHostName => RedisConnStr.Split(",").FirstOrDefault<string>()?.Split(":").FirstOrDefault();
        public string? RedisPort => RedisConnStr.Split(",").FirstOrDefault<string>()?.Split(":")[1];

        public AppConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
