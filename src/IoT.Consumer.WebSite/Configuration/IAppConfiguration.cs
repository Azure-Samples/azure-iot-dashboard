namespace Iot.PnpDashboard.Configuration
{
    public interface IAppConfiguration
    {
        string CheckpointStaConnString { get; set; }
        string CheckpointStaContainer { get; set; }
        string? CheckpointAccountName { get; set; }
        string IotHubConnStr { get; set; }
        string IotHubConsumerGroup { get; set; }
        string? IotHubHostName { get; set; }
        string SignalRConnStr { get; set; }
        string? SignalREndpoint { get; set; }
    }
}