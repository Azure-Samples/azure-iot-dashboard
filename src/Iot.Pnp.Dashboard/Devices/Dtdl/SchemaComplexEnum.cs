using System.Text.Json;
using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public enum EnumValueSchema //TODO: reference in enum definition for "Data Type" looks like not accurate.
    {
        [JsonPropertyName("integer")]
        Integer,
        [JsonPropertyName("string")]
        String
    }

    public class SchemaComplexEnum
    {
        [JsonPropertyName("@type")]
        public DtdlTypeEnum Type { get; set; }

        [JsonPropertyName("enumValues")]
        public List<EnumValue> EnumValues { get; set; } = new List<EnumValue>();

        [JsonPropertyName("valueSchema")]
        public EnumValueSchema ValueSchema { get; set; }

        [JsonPropertyName("@id")]
        public string? Id { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

    }

    public struct EnumValue
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("enumValue")]
        public JsonElement Value { get; set; }

        [JsonPropertyName("@id")]
        public string? Id { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }


        public string? GetAsString()
        {
            if (Value.ValueKind == JsonValueKind.String)
            {
                return Value.GetString();
            }
            else
            {
                return null;
            }
        }

        public int? GetAsInteger()
        {
            if (Value.ValueKind == JsonValueKind.Number)
            {
                return Value.GetInt32();
            }
            else
            {
                return null;
            }
        }
    }
}
