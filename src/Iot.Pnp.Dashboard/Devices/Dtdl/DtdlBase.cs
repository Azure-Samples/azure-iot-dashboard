using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class DtdlBase
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("@id")]
        public string? Id { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("description")]
        [JsonConverter(typeof(LocalizableStringConverter))]
        public Dictionary<string,string>? LocalizableDescription { get; set; }

        [JsonIgnore]
        public string? Description => GetLocalizedDescription();

        [JsonIgnore]
        public string? DisplayName => GetLocalizedDisplayName();


        [JsonPropertyName("displayName")]
        [JsonConverter(typeof(LocalizableStringConverter))]
        public Dictionary<string,string>? LocalizableDisplayName { get; set; }

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

    internal class LocalizableStringConverter : JsonConverter<Dictionary<string, string>>
    {
        public override bool CanConvert(Type t) => t == typeof(Dictionary<string, string>);

        public override Dictionary<string, string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Dictionary<string,string>? localizableStrings = new Dictionary<string,string>();
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                localizableStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader, options);
                if (localizableStrings != null)
                {
                    localizableStrings.Add("default", localizableStrings.ElementAt(0).Value);
                }
            }
            else
            {
                var s = JsonSerializer.Deserialize<string>(ref reader, options);
                if (s != null)
                {
                    localizableStrings.Add("default",s);
                }
            }
            return localizableStrings;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
        {
            if (value != null)
            {
                if (value.Count == 1)
                {
                    JsonSerializer.Serialize<string>(writer, value.ElementAt(0).Value, options);
                    return;
                }

                if (value.Count > 1)
                {
                    var valueCopy = new Dictionary<string, string>(value);
                    if (value.ContainsKey("default"))
                    {
                        valueCopy.Remove("default");
                    }
                    JsonSerializer.Serialize<Dictionary<string, string>>(writer, valueCopy, options);
                    return;
                }
            }

            throw new JsonException("Cannot marshal the property extends");

        }
    }


}
