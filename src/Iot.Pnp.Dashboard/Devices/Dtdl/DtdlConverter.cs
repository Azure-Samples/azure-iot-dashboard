using Iot.PnpDashboard.Devices.Dtdl;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    public class DtdlConverter
    {
        public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                SchemaConverter.Singleton,
                SchemaComplexConverter.Singleton,
                DtdlSemanticTypeConverter.Singleton,
                new JsonStringEnumConverter()
            },
        };
    }
}
