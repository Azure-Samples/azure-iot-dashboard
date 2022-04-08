using System.Text.Json;
using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class Telemetry : DtdlBase
    {
        [JsonPropertyName("@type")]
        public DtdlSemanticType Type { get; set; }

        [JsonPropertyName("schema")]
        public Schema Schema { get; set; }

        [JsonPropertyName("unit")]
        public string? Unit { get; set; }
    }
}
