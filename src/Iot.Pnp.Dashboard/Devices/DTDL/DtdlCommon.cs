using Azure.IoT.ModelsRepository;
using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DTMI : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DTMIList : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class IRI : Attribute
    {
    }

    public class DtdlCommon  
    {
        [DTMI]
        [JsonPropertyName("@id")]
        public string Id { get; set; }
        
        [IRI]
        [JsonPropertyName("@type")]
        public string Type { get; set; }
        
        [IRI]
        [JsonPropertyName("@context")]
        public string Context { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
    }
}
