using Microsoft.VisualStudio.TestTools.UnitTesting;
using Iot.PnpDashboard.Devices.Dtdl;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Azure;
using System.Text.Json;

namespace Iot.PnpDashboard.Tests
{
    [TestClass]
    public class DeserializeSchemaTests
    {
        internal class TestSchemaProperty
        {
            public Schema Schema { get; set; }
        }
        [TestMethod]
        public void Schema_Primitive_Test()
        {

            string json = "{\"schema\": \"double\"}";

            var deserialized = JsonSerializer.Deserialize<TestSchemaProperty>(json, DtdlConverter.Options);

            Assert.IsNull(deserialized.Schema.Complex);
            Assert.IsNotNull(deserialized.Schema.Primitive);
            Assert.IsTrue(deserialized.Schema.Primitive.HasValue);
            Assert.IsTrue(deserialized.Schema.IsPrimitive);
            Assert.IsFalse(deserialized.Schema.IsComplex);
            Assert.IsFalse(deserialized.Schema.IsGeoSpatial);
            Assert.AreEqual(deserialized.Schema.Primitive.Value, SchemaPrimitive.Double);
        }
        
        [TestMethod]
        public void Schema_GeoSpatial_Test()
        {
            string json = "{\"schema\": \"point\"}";

            var deserialized = JsonSerializer.Deserialize<TestSchemaProperty>(json, DtdlConverter.Options);

            Assert.IsNotNull(deserialized.Schema.GeoSpatial);
            Assert.IsNull(deserialized.Schema.Complex);
            Assert.IsNull(deserialized.Schema.Primitive);
            Assert.IsTrue(deserialized.Schema.GeoSpatial.HasValue);
            Assert.IsTrue(deserialized.Schema.IsGeoSpatial);
            Assert.IsFalse(deserialized.Schema.IsComplex);
            Assert.IsFalse(deserialized.Schema.IsPrimitive);
            Assert.AreEqual(deserialized.Schema.GeoSpatial.Value, SchemaGeoSpatial.Point);
        }

        [TestMethod]
        public void Schema_Complex_Array_Tests()
        {
            var json = "{   \"schema\": {        \"@type\": \"Array\",        \"elementSchema\": \"boolean\"    }   }";

            var deserialized = JsonSerializer.Deserialize<TestSchemaProperty>(json, DtdlConverter.Options);

            Assert.IsNotNull(deserialized);
            Assert.IsNull(deserialized.Schema.Primitive);
            Assert.IsNotNull(deserialized.Schema.Complex);
            Assert.IsTrue(deserialized.Schema.IsComplex);
            Assert.IsFalse(deserialized.Schema.IsPrimitive);
            Assert.IsFalse(deserialized.Schema.IsGeoSpatial);
            Assert.IsNotNull(deserialized.Schema.Complex?.Array);
            Assert.IsTrue(deserialized.Schema.Complex?.Array.ElementSchema.IsPrimitive);
            Assert.IsNull(deserialized.Schema.Complex?.Array.ElementSchema.Complex);
            Assert.AreEqual(deserialized.Schema.Complex?.Array.ElementSchema.Primitive, SchemaPrimitive.Boolean);
        }

        [TestMethod]
        public void Complex_Array_Test()
        {
            var json = "{   \"@type\": \"Array\",        \"elementSchema\": \"boolean\"    }";

            var deserialized = JsonSerializer.Deserialize<SchemaComplexArray>(json, DtdlConverter.Options);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(deserialized.Type, DtdlTypeEnum.Array);
            Assert.IsTrue(deserialized.ElementSchema.IsPrimitive);
            Assert.AreEqual(deserialized.ElementSchema.Primitive.Value, SchemaPrimitive.Boolean);
        }

        [TestMethod]
        public void Schema_Complex_Enum_Test()
        {
            string json = "{    \"schema\": {        \"@type\": \"Enum\",        \"valueSchema\": \"integer\",        \"enumValues\": [            {                \"name\": \"offline\",                \"displayName\": \"Offline\",                \"enumValue\": 1            },            {                \"name\": \"online\",                \"displayName\": \"Online\",                \"enumValue\": 2            }        ]    }}";

            var deserialized = JsonSerializer.Deserialize<TestSchemaProperty>(json, DtdlConverter.Options);

            Assert.IsNotNull(deserialized);
            Assert.IsNull(deserialized.Schema.Primitive);
            Assert.IsNotNull(deserialized.Schema.Complex);
            Assert.IsTrue(deserialized.Schema.IsComplex);
            Assert.IsFalse(deserialized.Schema.IsPrimitive);
            Assert.IsFalse(deserialized.Schema.IsGeoSpatial);
            Assert.IsNotNull(deserialized.Schema.Complex?.Enum);
            Assert.AreEqual(deserialized.Schema.Complex?.Enum.ValueSchema, EnumValueSchema.Integer);
            Assert.AreEqual(deserialized.Schema.Complex?.Enum.EnumValues.Count, 2);
            Assert.AreEqual(deserialized.Schema.Complex?.Enum.EnumValues[0].GetAsInteger(), 1);
            Assert.AreEqual(deserialized.Schema.Complex?.Enum.EnumValues[1].GetAsInteger(), 2);
        }

        [TestMethod]
        public void Schema_Complex_Enum_Disoredered_Test()
        {
            string json = "{    \"schema\": {        \"valueSchema\": \"integer\",        \"@type\": \"Enum\",        \"enumValues\": [            {                \"name\": \"offline\",                \"displayName\": \"Offline\",                \"enumValue\": 1            },            {                \"name\": \"online\",                \"displayName\": \"Online\",                \"enumValue\": 2            }        ]    }}";

            var deserialized = JsonSerializer.Deserialize<TestSchemaProperty>(json, DtdlConverter.Options);

            Assert.IsNotNull(deserialized);
            Assert.IsNull(deserialized.Schema.Primitive);
            Assert.IsNotNull(deserialized.Schema.Complex);
            Assert.IsTrue(deserialized.Schema.IsComplex);
            Assert.IsFalse(deserialized.Schema.IsPrimitive);
            Assert.IsFalse(deserialized.Schema.IsGeoSpatial);
            Assert.IsNotNull(deserialized.Schema.Complex?.Enum);
            Assert.AreEqual(deserialized.Schema.Complex?.Enum.ValueSchema, EnumValueSchema.Integer);
            Assert.AreEqual(deserialized.Schema.Complex?.Enum.EnumValues.Count, 2);
            Assert.AreEqual(deserialized.Schema.Complex?.Enum.EnumValues[0].GetAsInteger(), 1);
            Assert.AreEqual(deserialized.Schema.Complex?.Enum.EnumValues[1].GetAsInteger(), 2);
        }

        //"\"schema\": {        \"@type\": \"Object\",        \"fields\": [          {            \"name\": \"httpStreamTelemetry\",            \"displayName\": {              \"en\": \"HTTP Stream Telemetry\"            },            \"description\": {              \"en\": \"Enable or disable the HTTP telemetry stream to cloud.\"            },            \"schema\": {              \"@type\": \"Enum\",              \"valueSchema\": \"string\",              \"enumValues\": [                {                  \"name\": \"enabled\",                  \"enumValue\": \"enabled\",                  \"displayName\": {                    \"en\": \"Enabled\"                  }                },                {                  \"name\": \"disabled\",                  \"enumValue\": \"disabled\",                  \"displayName\": {                    \"en\": \"Disabled\"                  }                }              ]            }          }        ]      }"
        [TestMethod]
        public void Schema_Complex_Object_Disoredered_Test()
        {
            string json = "{    \"schema\": {        \"@type\": \"Object\",        \"fields\": [          {            \"name\": \"httpStreamTelemetry\",            \"displayName\": {              \"en\": \"HTTP Stream Telemetry\"            },            \"description\": {              \"en\": \"Enable or disable the HTTP telemetry stream to cloud.\"            },            \"schema\": {              \"@type\": \"Enum\",              \"valueSchema\": \"string\",              \"enumValues\": [                {                  \"name\": \"enabled\",                  \"enumValue\": \"enabled\",                  \"displayName\": {                    \"en\": \"Enabled\"                  }                },                {                  \"name\": \"disabled\",                  \"enumValue\": \"disabled\",                  \"displayName\": {                    \"en\": \"Disabled\"                  }                }              ]            }          }        ]      }}";

            var deserialized = JsonSerializer.Deserialize<TestSchemaProperty>(json, DtdlConverter.Options);

            Assert.IsNotNull(deserialized);
            Assert.IsNull(deserialized.Schema.Primitive);
            Assert.IsNotNull(deserialized.Schema.Complex);
            Assert.IsTrue(deserialized.Schema.IsComplex);
            Assert.IsFalse(deserialized.Schema.IsPrimitive);
            Assert.IsFalse(deserialized.Schema.IsGeoSpatial);
            Assert.IsNotNull(deserialized.Schema.Complex?.Object);
        }


        [TestMethod]
        [ExpectedException(typeof(JsonException))]
        public void Schema_Complex_Enum_WrongType_Test()
        {
            try
            {
                string json = "{    \"schema\": {        \"@type\": \"WrongType\",        \"valueSchema\": \"integer\",        \"enumValues\": [            {                \"name\": \"offline\",                \"displayName\": \"Offline\",                \"enumValue\": 1            },            {                \"name\": \"online\",                \"displayName\": \"Online\",                \"enumValue\": 2            }        ]    }}";

                var deserialized = JsonSerializer.Deserialize<TestSchemaProperty>(json, DtdlConverter.Options);
            }
            catch(JsonException ex)
            {
                Assert.IsTrue(ex.Message.Contains("WrongType"));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JsonException))]
        public void Schema_Complex_Enum_MissingType_Test()
        {
            string json = "{    \"schema\": {        \"@NOTYPE\": \"Enum\",        \"valueSchema\": \"integer\",        \"enumValues\": [            {                \"name\": \"offline\",                \"displayName\": \"Offline\",                \"enumValue\": 1            },            {                \"name\": \"online\",                \"displayName\": \"Online\",                \"enumValue\": 2            }        ]    }}";

            var deserialized = JsonSerializer.Deserialize<TestSchemaProperty>(json, DtdlConverter.Options);
        }

        [TestMethod]
        public void EnumValues_Integer_Test()
        {
            string json = "{            \"@type\": \"Enum\",            \"valueSchema\": \"integer\",            \"enumValues\": [                {                    \"name\": \"offline\",                    \"displayName\": \"Offline\",                    \"enumValue\": 1                },                {                    \"name\": \"online\",                    \"displayName\": \"Online\",                    \"enumValue\": 2                }            ]        }";

            var deserialized = JsonSerializer.Deserialize<SchemaComplexEnum>(json, DtdlConverter.Options);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(deserialized.Type, DtdlTypeEnum.Enum);
            Assert.AreEqual(deserialized.ValueSchema, EnumValueSchema.Integer);
            Assert.AreEqual(deserialized.EnumValues.Count, 2);
            Assert.AreEqual(deserialized.EnumValues[0].GetAsInteger(), 1);
            Assert.AreEqual(deserialized.EnumValues[1].GetAsInteger(), 2);
        }

        [TestMethod]
        public void EnumValues_String_Test()
        {
            string json = " {        \"@type\": \"Enum\",        \"valueSchema\": \"string\",        \"enumValues\": [            {                \"name\": \"offline\",                \"displayName\": \"Offline\",                \"enumValue\": \"offline\"            },            {                \"name\": \"online\",                \"displayName\": \"Online\",                \"enumValue\": \"online\"            }        ]    }";

            var deserialized = JsonSerializer.Deserialize<SchemaComplexEnum>(json, DtdlConverter.Options);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(deserialized.Type, DtdlTypeEnum.Enum);
            Assert.AreEqual(deserialized.ValueSchema, EnumValueSchema.String);
            Assert.AreEqual(deserialized.EnumValues.Count, 2);
            Assert.AreEqual(deserialized.EnumValues[0].GetAsString(), "offline");
            Assert.AreEqual(deserialized.EnumValues[1].GetAsString(), "online");
        }
    }
}