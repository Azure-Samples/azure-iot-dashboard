using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class Property
    {
        [JsonPropertyName("@type")]
        public DtdlType Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("schema")]
        public Schema Schema { get; set; }

        [JsonPropertyName("@id")]
        public string? Id { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; }

        [JsonPropertyName("writable")]
        public bool? Writable { get; set; }
    }
}