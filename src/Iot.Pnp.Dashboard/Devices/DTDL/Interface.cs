using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class Interface : DtdlCommon
    {
        public new string Type => "Interface";

        [JsonPropertyName("@contents")]
        public List<DtdlContent> Contents { get; set; }

        [DTMIList]
        [JsonPropertyName("extends")]
        public List<string> Extends { get; set; }

        [JsonPropertyName("extends")]
        public List<InterfaceSchema> Schemas { get; set; }
    }
}
