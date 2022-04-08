using Microsoft.Azure.Amqp.Framing;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class Interface : DtdlBase
    {
        [JsonPropertyName("@type")]
        public DtdlTypeEnum Type { get; set; }

        [JsonPropertyName("@context")]
        [JsonConverter(typeof(SingleStringOrListOfStringConverter))]
        public List<string> Context { get; set; }

        [JsonPropertyName("contents")]
        public Contents? Contents { get; set; }

        [JsonPropertyName("extends")]
        [JsonConverter(typeof(SingleStringOrListOfStringConverter))]
        public List<string>? Extends { get; set; }

        [JsonPropertyName("schemas")]
        public List<Schema>? Schemas { get; set; }
    }

    internal class SingleStringOrListOfStringConverter : JsonConverter<List<String>>
    {
        public override bool CanConvert(Type t) => t == typeof(List<String>);

        public override List<String>? Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options)
        {
            List<string>? extends = new List<string>();
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                extends = JsonSerializer.Deserialize<List<string>>(ref reader, options);

            }
            else
            {
                var s = JsonSerializer.Deserialize<string>(ref reader, options);
                if (s != null)
                {
                    extends.Add(s);
                }
            }
            return extends;
        }

        public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
        {
            if (value != null)
            {
                if (value.Count == 1)
                {
                    JsonSerializer.Serialize<string>(writer, value[0], options);
                    return;
                }

                if (value.Count > 1)
                {
                    JsonSerializer.Serialize<List<string>>(writer, value, options);
                    return;
                }
            }

            throw new JsonException("Cannot marshal the property extends");

        }
    }


}
