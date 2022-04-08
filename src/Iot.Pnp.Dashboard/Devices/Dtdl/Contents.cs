using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    [JsonConverter(typeof(ContentsConverter))]
    public class Contents
    {
        public List<Telemetry> Telemetry { get; set; }
        public List<Property> Properties { get; set; }
        public List<Command> Commands { get; set; }
        public List<Relationship> Relationships { get; set; }
        public List<Component> Components { get; set; }
    }

    internal class ContentsConverter : JsonConverter<Contents>
    {
        public override bool CanConvert(Type t) => t == typeof(Contents);

        public override Contents Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options)
        {
            Utf8JsonReader readerClone = reader;

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Cannot unmarshal type Contents");
            }

            JsonArray? elements = JsonSerializer.Deserialize<JsonArray>(ref reader, options);
            List<Telemetry> telemetry = new List<Telemetry>();
            List<Property> properties = new List<Property>();
            List<Component> components = new List<Component>();
            List<Relationship> relationships = new List<Relationship>();
            List<Command> commands = new List<Command>();

            foreach(JsonNode e in elements)
            {
                if (e.ToJsonString().Contains("Telemetry"))
                {
                    Telemetry? item = e.Deserialize<Telemetry>(options);
                    if (item != null)
                    {
                        telemetry.Add(item);
                    }
                }
                if (e.ToJsonString().Contains("Property"))
                {
                    Property? item = e.Deserialize<Property>(options);
                    if (item != null)
                    {
                        properties.Add(item);
                    }
                }
                if (e.ToJsonString().Contains("Component"))
                {
                    Component? item = e.Deserialize<Component>(options);
                    if (item != null)
                    {
                        components.Add(item);
                    }
                }
                if (e.ToJsonString().Contains("Relationship"))
                {
                    Relationship? item = e.Deserialize<Relationship>(options);
                    if (item != null)
                    {
                        relationships.Add(item);
                    }
                }
                if (e.ToJsonString().Contains("Command"))
                {
                    Command? item = e.Deserialize<Command>(options);
                    if (item != null)
                    {
                        commands.Add(item);
                    }
                }
            }
            return new Contents()
            {
                Telemetry = telemetry,
                Properties = properties,
                Components = components,
                Relationships = relationships,
                Commands = commands
            };

        }

        public override void Write(Utf8JsonWriter writer, Contents value, JsonSerializerOptions options)
        {
            List<object> elements = new List<object>();
            elements.AddRange(value.Telemetry);
            elements.AddRange(value.Properties);
            elements.AddRange(value.Components);
            elements.AddRange(value.Relationships);
            elements.AddRange(value.Commands);
            JsonSerializer.Serialize<List<object>>(elements, options);
        }
    }

}
