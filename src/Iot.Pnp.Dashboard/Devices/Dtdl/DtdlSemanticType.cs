using System.Text.Json.Serialization;
using System.Text.Json;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    [JsonConverter(typeof(DtdlSemanticTypeConverter))]
    public class DtdlSemanticType
    {
        public DtdlTypeEnum TypeName { get; set; }
        public string? SemanticType { get; set; } = null;

        public static implicit operator DtdlSemanticType(DtdlTypeEnum contentType) => new DtdlSemanticType() { TypeName = contentType };
    }

    internal class DtdlSemanticTypeConverter : JsonConverter<DtdlSemanticType>
    {
        public override bool CanConvert(Type t) => t == typeof(DtdlSemanticType);

        public override DtdlSemanticType Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options)
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
                return new DtdlSemanticType() { TypeName = dtdlContentType, SemanticType = semanticType };

            }
            else
            {
                throw new JsonException("Cannot unmarshal type DtdlContentType. The expected value for @type is not part of possible options: Telemetry, Property, Command, Relationship or Component.");
            }

            throw new JsonException("Cannot unmarshal type Type");
        }

        public override void Write(Utf8JsonWriter writer, DtdlSemanticType value, JsonSerializerOptions options)
        {
            if (value.SemanticType == null)
            {
                JsonSerializer.Serialize(writer, value.TypeName);
                return;
            }
            else
            {
                JsonSerializer.Serialize(writer, new List<string>() { value.TypeName.ToString(), value.SemanticType });
                return;
            }
            throw new JsonException("Cannot marshal type Type");
        }
    }
}
