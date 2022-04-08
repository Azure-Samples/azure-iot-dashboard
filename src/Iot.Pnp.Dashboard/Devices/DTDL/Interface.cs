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
        public string Context { get; set; }

        [JsonPropertyName("contents")]
        public Contents? Contents { get; set; }

        [JsonPropertyName("extends")]
        public List<string>? Extends { get; set; }

        [JsonPropertyName("schemas")]
        public List<Schema>? Schemas { get; set; }
    }

}
