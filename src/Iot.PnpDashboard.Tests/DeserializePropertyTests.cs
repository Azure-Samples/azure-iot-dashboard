using Microsoft.VisualStudio.TestTools.UnitTesting;
using Iot.PnpDashboard.Devices.Dtdl;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Azure;
using System.Text.Json;

namespace Iot.PnpDashboard.Tests
{
    [TestClass]
    public class DeserializePropertyTests
    {
        [TestMethod]
        public void Property_ComplexSchema_Test()
        {

            string json = "{    \"@type\": \"Property\",    \"name\": \"ledState\",    \"schema\": {        \"@type\": \"Array\",        \"elementSchema\": \"boolean\"    }, \"writable\":false}";

            var deserialized = JsonSerializer.Deserialize<Property>(json, DtdlConverter.Options);

            Assert.AreEqual(deserialized.Name, "ledState");
            Assert.AreEqual(deserialized.Type.Content, DtdlTypeEnum.Property);
            Assert.IsNull(deserialized.Id);
            Assert.IsNull(deserialized.Unit);
            Assert.IsNull(deserialized.Description);
            Assert.IsNull(deserialized.Comment);
            Assert.IsNull(deserialized.DisplayName);
            Assert.IsNotNull(deserialized.Schema.Complex);
            Assert.IsFalse(deserialized.Schema.IsPrimitive);
            Assert.IsTrue(deserialized.Schema.IsComplex);
            Assert.IsFalse(deserialized.Writable);
        }

        [TestMethod]
        public void Property_PrimitiveSchema_Test()
        {

            string json = "{    \"@type\": \"Property\",    \"name\": \"setPointTemp\",    \"schema\": \"double\",    \"writable\": true}";

            var deserialized = JsonSerializer.Deserialize<Property>(json, DtdlConverter.Options);

            Assert.AreEqual(deserialized.Name, "setPointTemp");
            Assert.AreEqual(deserialized.Type.Content, DtdlTypeEnum.Property);
            Assert.IsNull(deserialized.Id);
            Assert.IsNull(deserialized.Unit);
            Assert.IsNull(deserialized.Description);
            Assert.IsNull(deserialized.Comment);
            Assert.IsNull(deserialized.DisplayName);
            Assert.IsTrue(deserialized.Schema.IsPrimitive);
            Assert.IsNull(deserialized.Schema.Complex);
            Assert.IsFalse(deserialized.Schema.IsComplex);
            Assert.IsTrue(deserialized.Writable);
        }

        [TestMethod]
        public void Property_SemanticType_Test()
        {

            string json = "{    \"@type\": [\"Property\", \"Temperature\"],    \"name\": \"setPointTemp\",    \"schema\": \"double\",    \"unit\": \"degreeCelsius\",    \"writable\": true}";

            var deserialized = JsonSerializer.Deserialize<Property>(json, DtdlConverter.Options);

            Assert.AreEqual(deserialized.Name, "setPointTemp");
            Assert.AreEqual(deserialized.Type.Content, DtdlTypeEnum.Property);
            Assert.AreEqual(deserialized.Type.SemanticType, "Temperature");
            Assert.IsNull(deserialized.Id);
            Assert.AreEqual(deserialized.Unit, "degreeCelsius");
            Assert.IsNull(deserialized.Description);
            Assert.IsNull(deserialized.Comment);
            Assert.IsNull(deserialized.DisplayName);
            Assert.IsTrue(deserialized.Schema.IsPrimitive);
            Assert.IsNull(deserialized.Schema.Complex);
            Assert.IsFalse(deserialized.Schema.IsComplex);
            Assert.IsTrue(deserialized.Writable);
        }
    }
}