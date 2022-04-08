using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class Command : DtdlBase
    {
        [JsonPropertyName("@type")]
        public DtdlTypeEnum Type { get; set; }

        [JsonPropertyName("commandType")]
        [Obsolete("Deprected in the DTDLv2 Model.", true)]
        public string CommandType { get; set; }

        [JsonPropertyName("request")]
        public CommandPayload Request{ get; set; }

        [JsonPropertyName("response")]
        public CommandPayload Response { get; set; }
    }
}