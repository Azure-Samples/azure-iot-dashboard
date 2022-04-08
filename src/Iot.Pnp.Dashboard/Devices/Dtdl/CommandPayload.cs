using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class CommandPayload : DtdlBase
    {
        [JsonPropertyName("schema")]
        public Schema Schema { get; set; }
    }
}