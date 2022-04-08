using Microsoft.Azure.Amqp.Framing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class Interface
    {
        [JsonPropertyName("@id")]
        public string Id { get; set; }

        [JsonPropertyName("@type")]
        public string Type { get; set; }

        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("contents")]
        public Contents? Contents { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("extends")]
        public List<string> Extends { get; set; }

        [JsonPropertyName("extends")]
        public List<Schema> Schemas { get; set; }
    }

    public class Contents
    {
        List<Telemetry> Telemetry;
        List<Property> Properties;
        List<Command> Commands;
        List<Relationship> Relationships;
        List<Component> Components;
    }

    //internal class ContentsConverter : JsonConverter<Contents>
    //{
    //    public override bool CanConvert(Type t) => t == typeof(Contents);

    //    public override Contents Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options)
    //    {
    //        Utf8JsonReader readerClone = reader;

    //        while (readerClone.Read() && readerClone.TokenType != JsonTokenType.EndObject)
    //        {
    //            if (readerClone.TokenType == JsonTokenType.PropertyName)
    //            {
    //                string? propertyName = readerClone.GetString();
    //                if (propertyName == "@type")
    //                {
    //                    readerClone.Read();
    //                    if (readerClone.TokenType == JsonTokenType.String)
    //                    {
    //                        string complexTypeStr = readerClone.GetString()!;
    //                        if (Enum.TryParse(complexTypeStr, true, out complexType))
    //                        {
    //                            break;
    //                        }
    //                        else
    //                        {
    //                            throw new JsonException($"Unexpected parsed schema @type '{ complexTypeStr }'");
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        switch (complexType)
    //        {
    //            case (ContentsType.Array):
    //                var arrayObject = JsonSerializer.Deserialize<ContentsArray>(ref reader, options);
    //                return new Contents() { Array = arrayObject };
    //            case (ContentsType.Enum):
    //                var enumObject = JsonSerializer.Deserialize<ContentsEnum>(ref reader, options);
    //                return new Contents() { Enum = enumObject };
    //            case (ContentsType.Map):
    //                var mapObject = JsonSerializer.Deserialize<ContentsMap>(ref reader, options);
    //                return new Contents() { Map = mapObject };
    //            case (ContentsType.Object):
    //                var objectObject = JsonSerializer.Deserialize<ContentsObject>(ref reader, options);
    //                return new Contents() { Object = objectObject };
    //            default:
    //                throw new JsonException("Cannot unmarshal type Contents");
    //        }
    //    }

    //    public override void Write(Utf8JsonWriter writer, Contents value, JsonSerializerOptions options)
    //    {
    //        if (value.IsArray)
    //        {
    //            JsonSerializer.Serialize(writer, value.Array);
    //            return;
    //        }
    //        if (value.IsEnum)
    //        {
    //            JsonSerializer.Serialize(writer, value.Enum);
    //            return;
    //        }
    //        if (value.IsMap)
    //        {
    //            JsonSerializer.Serialize(writer, value.Map);
    //            return;
    //        }
    //        if (value.IsObject)
    //        {
    //            JsonSerializer.Serialize(writer, value.Object);
    //            return;
    //        }
    //        throw new JsonException("Cannot marshal type Contents");
    //    }

    //    public static readonly ContentsConverter Singleton = new ContentsConverter();
    //}

}
