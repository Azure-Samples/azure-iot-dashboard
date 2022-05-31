using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class SchemaComplexObject
    {
        [JsonPropertyName("@type")]
        public DtdlTypeEnum Type { get; set; }

        [JsonPropertyName("fields")]
        public List<Field> Fields { get; set; }

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

    //TODO: review sacar base para name, id, comment, ds

    public class Field : DtdlBase
    {

        [JsonPropertyName("schema")]
        public Schema Schema { get; set; }

    }
}
