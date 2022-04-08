using Microsoft.VisualStudio.TestTools.UnitTesting;
using Iot.PnpDashboard.Devices.Dtdl;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Azure;
using System.Text.Json;

namespace Iot.PnpDashboard.Tests
{
    [TestClass]
    public class DeserializeTelemetryTests
    {
        [TestMethod]
        public void Telemetry_PrimitiveSchema_Test()
        {

            string json = "{    \"@type\": \"Telemetry\",    \"name\": \"temp\",    \"schema\": \"double\"}";

            var deserialized = JsonSerializer.Deserialize<Telemetry>(json, DtdlConverter.Options);

            Assert.AreEqual(deserialized.Name, "temp");
            Assert.AreEqual(deserialized.Type.TypeName, DtdlTypeEnum.Telemetry);
            Assert.IsNull(deserialized.Type.SemanticType);
            Assert.IsNull(deserialized.Id);
            Assert.IsNull(deserialized.Unit);
            Assert.IsNull(deserialized.Description);
            Assert.IsNull(deserialized.Comment);
            Assert.IsNull(deserialized.DisplayName);
            Assert.IsTrue(deserialized.Schema.IsPrimitive);
            Assert.IsFalse(deserialized.Schema.IsComplex);
            Assert.AreEqual(deserialized.Schema.Primitive, SchemaPrimitive.Double);
        }

        [TestMethod]
        public void Telemetry_ComplexSchema_Test()
        {

            string json = "{    \"@type\": \"Telemetry\",    \"name\": \"ledState\",    \"schema\": {        \"@type\": \"Array\",        \"elementSchema\": \"boolean\"    }}";

            var deserialized = JsonSerializer.Deserialize<Telemetry>(json, DtdlConverter.Options);

            Assert.AreEqual(deserialized.Name, "ledState");
            Assert.AreEqual(deserialized.Type.TypeName, DtdlTypeEnum.Telemetry);
            Assert.IsNull(deserialized.Type.SemanticType);
            Assert.IsNull(deserialized.Id);
            Assert.IsNull(deserialized.Unit);
            Assert.IsNull(deserialized.Description);
            Assert.IsNull(deserialized.Comment);
            Assert.IsNull(deserialized.DisplayName);
            Assert.IsNotNull(deserialized.Schema.Complex);
            Assert.IsFalse(deserialized.Schema.IsPrimitive);
            Assert.IsTrue(deserialized.Schema.IsComplex);
        }

        [TestMethod]
        public void Telemetry_SemanticType_Test()
        {

            string json = "{    \"@type\": [\"Telemetry\", \"Temperature\"],    \"name\": \"temp\",    \"schema\": \"double\",    \"unit\": \"degreeCelsius\"}";

            var deserialized = JsonSerializer.Deserialize<Telemetry>(json, DtdlConverter.Options);

            Assert.AreEqual(deserialized.Name, "temp");
            Assert.AreEqual(deserialized.Type.TypeName, DtdlTypeEnum.Telemetry);
            Assert.IsNotNull(deserialized.Type.SemanticType);
            Assert.AreEqual(deserialized.Type.SemanticType, "Temperature");
            Assert.IsNull(deserialized.Id);
            Assert.IsNotNull(deserialized.Unit);
            Assert.AreEqual(deserialized.Unit, "degreeCelsius");
            Assert.IsNull(deserialized.Description);
            Assert.IsNull(deserialized.Comment);
            Assert.IsNull(deserialized.DisplayName);
            Assert.IsNull(deserialized.Schema.Complex);
            Assert.IsTrue(deserialized.Schema.IsPrimitive);
            Assert.IsFalse(deserialized.Schema.IsComplex);
        }
    }
}