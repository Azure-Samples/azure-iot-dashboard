using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{

    public enum SchemaPrimitive
    {
        //Primitive
        [JsonPropertyName("boolean")]
        Boolean,
        [JsonPropertyName("date")]
        Date,
        [JsonPropertyName("dateTime")]
        DateTime,
        [JsonPropertyName("double")]
        Double,
        [JsonPropertyName("duration")]
        Duration,
        [JsonPropertyName("float")]
        Float,
        [JsonPropertyName("integer")]
        Integer,
        [JsonPropertyName("long")]
        Long,
        [JsonPropertyName("string")]
        String,
        [JsonPropertyName("time")]
        Time,
    }

    public enum SchemaGeoSpatial 
    { 
        //GeoSpatial
        [JsonPropertyName("point")]
        Point,
        [JsonPropertyName("multiPoint")]
        MultiPoint,
        [JsonPropertyName("lineString")]
        LineString,
        [JsonPropertyName("multiLineString")]
        MultiLineString,
        [JsonPropertyName("polygon")]
        Polygon,
        [JsonPropertyName("multiPolygon")]
        MultiPolygon

    }
}
