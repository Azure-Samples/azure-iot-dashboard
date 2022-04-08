using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class Property : Telemetry
    {
        [JsonPropertyName("writable")]
        public bool? Writable { get; set; }
    }
}