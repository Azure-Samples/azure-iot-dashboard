using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class InterfaceSchema
    {
        [DTMI]
        [JsonPropertyName("@id")]
        public string Id { get; set; }

        [IRI]
        [JsonPropertyName("@type")]
        public string Type { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
    }
}
