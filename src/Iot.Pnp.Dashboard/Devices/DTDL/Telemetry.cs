using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class Telemetry : DtdlCommon
    {
        public new string[] Type => new string[2] { "Telemetry", "" }; //TODO: Validate if "@type": ["Telemetry", ""], is valid DTDL

        [JsonPropertyName("schema")]
        public Schema Schema { get; set; }

        public string Unit { get; set; }
    }
}
