using Azure.Messaging.EventHubs.Consumer;

namespace Iot.PnpDashboard.Configuration
{
    public class AppConfiguration : IAppConfiguration
    {
        public string IotHubConnStr { get; set; }
        public string? IotHubHostName { get; set; }
        public string IotHubConsumerGroup { get; set; }
        public string CheckpointStaConnString { get; set; }
        public string CheckpointStaContainer { get; set; }
        public string? CheckpointAccountName { get; set; }
        public string SignalRConnStr { get; set; }
        public string? SignalREndpoint { get; set; }

        public AppConfiguration(IConfiguration configuration)
        {
            IotHubConnStr = configuration.GetValue<String>("Azure:IotHub:ConnectionString");
            IotHubConsumerGroup = configuration.GetValue<String>("Azure:IotHub:ConsumerGroup") ?? EventHubConsumerClient.DefaultConsumerGroupName;
            IotHubHostName = IotHubConnStr.Split(';').Where(x => x.Contains("HostName")).Select(s => s.Split('=').LastOrDefault()).FirstOrDefault();
            CheckpointStaConnString = configuration.GetValue<String>("Azure:CheckpointStorageAccount:ConnectionString");
            CheckpointStaContainer = configuration.GetValue<String>("Azure:CheckpointStorageAccount:Container") ?? "iot-hub-checkpointing";
            CheckpointAccountName = CheckpointStaConnString.Split(';').Where(x => x.Contains("AccountName")).Select(s => s.Split('=').LastOrDefault()).FirstOrDefault();
            SignalRConnStr = configuration.GetValue<String>("Azure:SignalR:ConnectionString");
            SignalREndpoint = SignalRConnStr.Split(';').Where(x => x.Contains("Endpoint")).Select(s => s.Split('=').LastOrDefault()).FirstOrDefault();
        }
    }
}
