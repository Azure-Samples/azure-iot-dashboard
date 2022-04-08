using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public enum DtdlTypeEnum
    {
        [JsonIgnore]
        Unknown = 0,
        Interface,
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


}
