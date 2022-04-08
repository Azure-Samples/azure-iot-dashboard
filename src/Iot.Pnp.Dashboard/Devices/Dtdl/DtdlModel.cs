using System.Text.Json;
using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class DtdlModel
    {
        public List<Interface> Interfaces { get; set; }
    }

    public class DtdlModelConverter : JsonConverter<DtdlModel>
    {
        public override bool CanConvert(Type t) => t == typeof(DtdlModel);

        public override DtdlModel? Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options)
        {
            List<Interface>? interfaces = new List<Interface>();
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                interfaces = JsonSerializer.Deserialize<List<Interface>>(ref reader, options);

            }
            else
            {
                var i = JsonSerializer.Deserialize<Interface>(ref reader, options);
                if (i != null)
                {
                    interfaces.Add(i);
                }
            }
            return new DtdlModel() { Interfaces = interfaces };
        }

        public override void Write(Utf8JsonWriter writer, DtdlModel value, JsonSerializerOptions options)
        {
            if (value.Interfaces != null)
            {
                if (value.Interfaces.Count == 1)
                {
                    JsonSerializer.Serialize<Interface>(value.Interfaces[0], options);
                    return;
                }

                if (value.Interfaces.Count > 1)
                {
                    JsonSerializer.Serialize<List<Interface>>(value.Interfaces, options);
                    return;
                }
            }

            throw new JsonException("Cannot marshal type DtdModel. Intefaces is null or empty.");

        }
        public static readonly DtdlModelConverter Singleton = new DtdlModelConverter();
    }
}
