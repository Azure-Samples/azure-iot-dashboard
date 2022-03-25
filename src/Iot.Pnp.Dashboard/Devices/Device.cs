using Microsoft.Azure.Devices.Shared;
using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices
{
    public class Device
    {
        public string DeviceId { get; set; } = string.Empty;
        public string ModelId { get; set; } = string.Empty;
        public DateTimeOffset? TelemetryTimestamp { get; set; }
        public string? LastOperation { get; set; }
        public DateTimeOffset? OperationTimestamp { get; set; }
        public string? OperationSource { get; set; }
        [JsonIgnore]
        public bool? Disconnected { get; set; }
        public DateTimeOffset TelemetryProcessorOffset { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset TelemetryBroadcastOffset { get; set; } = DateTimeOffset.UtcNow;
    }
}
