using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public enum PrimitiveSchemaEnum
    {
        [JsonPropertyName("boolean")]
        Boolean,
        [JsonPropertyName("date")]
        Date,
        [JsonPropertyName("dateTime")]
        DateTime,
        [JsonPropertyName("double")]
        Double,
        [JsonPropertyName("duration")]
        Duration,
        [JsonPropertyName("float")]
        Float,
        [JsonPropertyName("integer")]
        Integer,
        [JsonPropertyName("long")]
        Long,
        [JsonPropertyName("string")]
        String,
        [JsonPropertyName("time")]
        Time
    }
    public class PrimitiveSchema : Schema
    {
        public PrimitiveSchemaEnum Value; 
    }
}
