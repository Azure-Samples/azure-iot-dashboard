using Azure.Messaging.EventHubs.Consumer;
using Iot.PnpDashboard.Extensions;

namespace Iot.PnpDashboard.Configuration
{
    public class AppConfiguration
    {
        private readonly IConfiguration _configuration;

        public bool ManagedIdentityEnabled => _configuration.GetValueOrDefault<bool>("Azure:ManagedIdentity:Enabled", false);
        public string ManagedIdentityClientId => _configuration.GetValueOrThrow<string>("Azure:ManagedIdentity:ClientId");
        public string IotHubConnStr => _configuration.GetValueOrThrow<string>("Azure:IotHub:ConnectionString");
        public string? IotHubHostName => IotHubConnStr.Split(';').Where(x => x.Contains("HostName")).Select(s => s.Split('=').LastOrDefault()).FirstOrDefault();
        public string IotHubConsumerGroup => _configuration.GetValueOrDefault<string>("Azure:CheckpointStorageAccount:Container", EventHubConsumerClient.DefaultConsumerGroupName);
        public string CheckpointStaConnString => _configuration.GetValueOrThrow<string>("Azure:CheckpointStorageAccount:ConnectionString");
        public string CheckpointStaContainer => _configuration.GetValueOrDefault<string>("Azure:CheckpointStorageAccount:Container", "iot-hub-checkpointing");
        public string CheckpointStaAccountName => _configuration.GetValueOrThrow<string>("Azure:CheckpointStorageAccount:AccountName");
        public string SignalRConnStr => _configuration.GetValueOrThrow<string>("Azure:SignalR:ConnectionString");
        public string? SignalREndpoint => SignalRConnStr.Split(';').Where(x => x.Contains("Endpoint")).Select(s => s.Split('=').LastOrDefault()).FirstOrDefault();

        public AppConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
