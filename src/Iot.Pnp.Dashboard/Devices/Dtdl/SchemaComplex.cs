using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Net.Http.Headers;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using System.Reactive.Disposables;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Iot.PnpDashboard.Devices.Dtdl
{ 
    
    [JsonConverter(typeof(SchemaComplexConverter))]
    public struct SchemaComplex
    {
        public SchemaComplexArray? Array { get; set; } = null;
        public SchemaComplexEnum? Enum { get; set; } = null;
        public SchemaComplexMap? Map { get; set; } = null;
        public SchemaComplexObject? Object { get; set; } = null;

        public static implicit operator SchemaComplex(SchemaComplexArray arrayParam) => new SchemaComplex { Array = arrayParam };
        public static implicit operator SchemaComplex(SchemaComplexEnum enumParam) => new SchemaComplex { Enum = enumParam };
        public static implicit operator SchemaComplex(SchemaComplexMap arrayParam) => new SchemaComplex { Map = arrayParam };
        public static implicit operator SchemaComplex(SchemaComplexObject enumParam) => new SchemaComplex { Object = enumParam };

        public bool IsArray => Array != null;
        public bool IsEnum => Enum != null;
        public bool IsMap => Map != null;
        public bool IsObject => Object != null;
    }

    internal class SchemaComplexConverter : JsonConverter<SchemaComplex>
    {
        public override bool CanConvert(Type t) => t == typeof(SchemaComplex) || t == typeof(SchemaComplex?);

        public override SchemaComplex Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions options)
        {
            Utf8JsonReader readerClone = reader;

            DtdlTypeEnum complexType = DtdlTypeEnum.Unknown;

            while(readerClone.Read() && readerClone.TokenType != JsonTokenType.EndObject)
            {
                if (readerClone.TokenType == JsonTokenType.PropertyName)
                {
                    string? propertyName = readerClone.GetString();
                    if (propertyName == "@type")
                    {
                        readerClone.Read();
                        if (readerClone.TokenType == JsonTokenType.String)
                        {
                            string complexTypeStr = readerClone.GetString()!;
                            if(Enum.TryParse(complexTypeStr, true, out complexType))
                            {
                                break;
                            }
                            else
                            {
                                throw new JsonException($"Unexpected parsed schema @type '{ complexTypeStr }'");
                            }
                        }
                    }
                }
            }

            switch (complexType)
            {
                case (DtdlTypeEnum.Array):
                    var arrayObject = JsonSerializer.Deserialize<SchemaComplexArray>(ref reader, options);
                    return new SchemaComplex() { Array = arrayObject };
                case (DtdlTypeEnum.Enum):
                    var enumObject = JsonSerializer.Deserialize<SchemaComplexEnum>(ref reader, options);
                    return new SchemaComplex() { Enum = enumObject };
                case (DtdlTypeEnum.Map):
                    var mapObject = JsonSerializer.Deserialize<SchemaComplexMap>(ref reader, options);
                    return new SchemaComplex() { Map = mapObject };
                case (DtdlTypeEnum.Object):
                    var objectObject = JsonSerializer.Deserialize<SchemaComplexObject>(ref reader, options);
                    return new SchemaComplex() { Object = objectObject };
                default:
                    throw new JsonException("Cannot unmarshal type SchemaComplex");
            }
        }

        public override void Write(Utf8JsonWriter writer, SchemaComplex value, JsonSerializerOptions options)
        {
            if (value.IsArray)
            {
                JsonSerializer.Serialize(writer, value.Array);
                return;
            }
            if (value.IsEnum)
            {
                JsonSerializer.Serialize(writer, value.Enum);
                return;
            }
            if (value.IsMap)
            {
                JsonSerializer.Serialize(writer, value.Map);
                return;
            }
            if (value.IsObject)
            {
                JsonSerializer.Serialize(writer, value.Object);
                return;
            }
            throw new JsonException("Cannot marshal type SchemaComplex");
        }
    }
}
