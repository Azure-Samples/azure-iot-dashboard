using System.Text.Json;
using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class Telemetry 
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
    }

    public enum DtdlTypeEnum
    {
        [JsonIgnore]
        Unknown = 0,
        Inteface,
        Telemetry,
        Property,
        Command,
        Relationship,
        Component,
        Array,
        Enum,
        Map,
        Object
    }
    public class DtdlType
    {
        public DtdlTypeEnum Content { get; set; }
        public string? SemanticType { get; set; } = null;

        public static implicit operator DtdlType(DtdlTypeEnum contentType) => new DtdlType() { Content = contentType };
    }

    internal class DtdlTypeConverter : JsonConverter<DtdlType>
    {
        public override bool CanConvert(Type t) => t == typeof(DtdlType);

        public override DtdlType Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options)
        {
            Utf8JsonReader cloneReader = reader;
            string contentType = String.Empty;
            string? semanticType = null;

            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    contentType = reader.GetString()!;
                    break;
                case JsonTokenType.StartArray:
                    var typeData = JsonSerializer.Deserialize<List<string>>(ref reader, options);
                    if (typeData != null)
                    {
                        contentType = typeData[0];
                        semanticType = typeData[1];
                    }
                    break;
            }

            if (Enum.TryParse<DtdlTypeEnum>(contentType, true, out var dtdlContentType))
            {
                return new DtdlType() { Content = dtdlContentType, SemanticType = semanticType };
                
            }
            else
            {
                throw new JsonException("Cannot unmarshal type DtdlContentType. The expected value for @type is not part of possible options: Telemetry, Property, Command, Relationship or Component.");
            }

            throw new JsonException("Cannot unmarshal type Type");
        }

        public override void Write(Utf8JsonWriter writer, DtdlType value, JsonSerializerOptions options)
        {
            if (value.SemanticType == null)
            {
                JsonSerializer.Serialize(writer, value.Content);
                return;
            }
            else 
            { 
                JsonSerializer.Serialize(writer, new List<string>() { value.Content.ToString(), value.SemanticType });
                return;
            }
            throw new JsonException("Cannot marshal type Type");
        }

        public static readonly DtdlTypeConverter Singleton = new DtdlTypeConverter();
    }
}
