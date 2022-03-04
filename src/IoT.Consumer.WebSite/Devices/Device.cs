using Microsoft.Azure.Devices.Shared;

namespace Iot.PnpDashboard.Devices
{
    public class Device
    {
        public string DeviceId { get; set; } = string.Empty;
        public string ModelId { get; set; } = string.Empty;
        public string? MessageSource { get; set; }
        public DateTimeOffset? LastTelemetryTimestamp { get; set; }
        public string? LastOperation { get; set; }
        public DateTimeOffset? LastOperationTimestamp { get; set; }
        public bool? Disconnected { get; set; }
    }
}
