using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace Iot.PnpDashboard.Devices.Dtdl
{
    [JsonConverter(typeof(SchemaConverter))]
    public struct Schema
    {
        public SchemaPrimitive? Primitive { get; set; } = null;
        public SchemaComplex? Complex { get; set; } = null;
        public SchemaGeoSpatial? GeoSpatial { get; set; } = null;
        public string ReferenceId { get; set; } = null;

        public static implicit operator Schema(SchemaPrimitive schemaPrimitive) => new Schema { Primitive = schemaPrimitive };
        public static implicit operator Schema(SchemaComplex schemaComplex) => new Schema { Complex = schemaComplex };
        public static implicit operator Schema(SchemaGeoSpatial schemaGeoSpatial) => new Schema { GeoSpatial = schemaGeoSpatial };
        public static implicit operator Schema(string schemaId) => new Schema { ReferenceId = schemaId };

        public bool IsPrimitive => Primitive != null;
        public bool IsComplex => Complex != null;
        public bool IsGeoSpatial => GeoSpatial != null;
        public bool IsReferenceId => ReferenceId != null;

        public Schema()
        {
        }
    }

    internal class SchemaConverter : JsonConverter<Schema>
    {
        Regex DtmiRegex = new Regex (@"(?<schema>dtmi):(?<path>((?<segment>[_a-zA-Z][_a-zA-Z0-9]+[^_:;])*[:]|(?<segment>[_a-zA-Z][_a-zA-Z0-9]+[^_:;])*)+)[;][\d]+");
        public override bool CanConvert(Type t) =>  t == typeof(Schema) || t == typeof(Schema?);

        public override Schema Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options)
        {
            Utf8JsonReader cloneReader = reader;

            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    string stringValue = reader.GetString()!;

                    if (Enum.TryParse<SchemaPrimitive>(stringValue, true, out var primitive))
                    {
                        return new Schema { Primitive = primitive };
                    }
                    else if (Enum.TryParse<SchemaGeoSpatial>(stringValue, true, out var geoSpatial))
                    {
                        return new Schema { GeoSpatial = geoSpatial };
                    }
                    else if (DtmiRegex.IsMatch(stringValue))
                    {
                        return new Schema { ReferenceId = stringValue };
                    }
                    else
                    {
                        
                        throw new JsonException("Cannot unmarshal type Schema. The schema value is not a complex schema, not a valid DTMI schema reference or is not part of the Primitive or GeoSpatial options.");
                    }
                case JsonTokenType.StartObject:
                    var objectValue = JsonSerializer.Deserialize<SchemaComplex>(ref reader, options);
                    return new Schema{ Complex = objectValue };
            }
            throw new JsonException("Cannot unmarshal type Schema");
        }

        //TODO: Repasar Write para ver que usan el write...
        public override void Write(Utf8JsonWriter writer, Schema value, JsonSerializerOptions options)
        {
            if (value.Primitive != null)
            {
                JsonSerializer.Serialize(writer, value.Primitive.Value, options);
                return;
            }
            if (value.Complex != null)
            {
                JsonSerializer.Serialize(writer, value.Complex, options);
                return;
            }
            throw new JsonException("Cannot marshal type Schema");
        }
    }
}