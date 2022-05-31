using System.Diagnostics.Eventing.Reader;
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
        public string Id { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("description")]
        [JsonConverter(typeof(LocalizableStringConverter))]
        public Dictionary<string, string>? LocalizableDescription { get; set; }

        [JsonIgnore]
        public string? Description => GetLocalizedDescription();

        [JsonIgnore]
        public string? DisplayName => GetLocalizedDisplayName();


        [JsonPropertyName("displayName")]
        [JsonConverter(typeof(LocalizableStringConverter))]
        public Dictionary<string, string>? LocalizableDisplayName { get; set; }

        public string? GetLocalizedDescription(string languageCode = "default")
        {
            if (LocalizableDescription != null)
            {
                return LocalizableDescription[languageCode];
            }
            else
            {
                return null;
            }
        }

        public string? GetLocalizedDisplayName(string languageCode = "default")
        {
            if (LocalizableDisplayName != null)
            {
                return LocalizableDisplayName[languageCode];
            }
            else
            {
                return null;
            }
        }

    }

    public class EnumValue : DtdlBase
    {
        [JsonPropertyName("enumValue")]
        public JsonElement Value { get; set; }

        public bool IsString => Value.ValueKind == JsonValueKind.String;
        public bool IsInteger => Value.ValueKind == JsonValueKind.Number;

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
