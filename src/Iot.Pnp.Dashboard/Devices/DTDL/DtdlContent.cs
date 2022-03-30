using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class DtdlContent : DtdlCommon
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
