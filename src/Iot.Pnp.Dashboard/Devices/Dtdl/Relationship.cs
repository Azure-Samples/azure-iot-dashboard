using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class Relationship : Component
    {
        [JsonPropertyName("maxMultiplicity")]
        public int? MaxMultiplicity { get; set; }

        [JsonPropertyName("minMultiplicity")]
        public int? MinMultiplicity { get; set; }

        [JsonPropertyName("properties")]
        public List<Property>? Properties { get; set; }

        [JsonPropertyName("target")]
        public string? Target { get; set; }

        [JsonPropertyName("writable")]
        public bool? Writable { get; set; }
    }
}