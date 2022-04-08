using Microsoft.VisualStudio.TestTools.UnitTesting;
using Iot.PnpDashboard.Devices.Dtdl;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Azure;
using System.Text.Json;

namespace Iot.PnpDashboard.Tests
{
    [TestClass]
    public class DeserializeInterfaceTests
    {
        [TestMethod]
        public void Interface_Test()
        {

            string json = "{    \"@id\": \"dtmi:com:example:Thermostat;1\",    \"@type\": \"Interface\",    \"displayName\": \"Thermostat\",    \"contents\": [        {            \"@type\": \"Telemetry\",            \"name\": \"temp\",            \"schema\": \"double\"        },        {            \"@type\": \"Property\",            \"name\": \"setPointTemp\",            \"writable\": true,            \"schema\": \"double\"        }    ],    \"@context\": \"dtmi:dtdl:context;2\"}";

            var deserialized = JsonSerializer.Deserialize<Interface>(json, DtdlConverter.Options);

            Assert.AreEqual(deserialized.Id, "dtmi:com:example:Thermostat;1");
            Assert.AreEqual(deserialized.DisplayName, "Thermostat");
            Assert.AreEqual(deserialized.Type, DtdlTypeEnum.Interface);
            Assert.AreEqual(deserialized.Context, "dtmi:dtdl:context;2");
            Assert.IsNotNull(deserialized.Contents);
            Assert.IsNotNull(deserialized.Contents.Properties);
            Assert.AreEqual(deserialized.Contents.Properties.Count, 1);
            Assert.IsTrue(deserialized.Contents.Properties[0].Writable);
            Assert.AreEqual(deserialized.Contents.Properties[0].Name, "setPointTemp");
            Assert.AreEqual(deserialized.Contents.Properties[0].Schema.Primitive, SchemaPrimitive.Double);
            Assert.AreEqual(deserialized.Contents.Properties[0].Type.TypeName, DtdlTypeEnum.Property);
            Assert.IsNull(deserialized.Contents.Properties[0].Type.SemanticType);
            Assert.IsNotNull(deserialized.Contents.Telemetry);
            Assert.IsNotNull(deserialized.Contents.Commands);
            Assert.IsNotNull(deserialized.Contents.Relationships);
            Assert.IsNotNull(deserialized.Contents.Components);
            Assert.IsNull(deserialized.Comment);
        }

        [TestMethod]
        public void Interface_Components_Test()
        {

            string json = "{    \"@id\": \"dtmi:com:example:Phone;2\",    \"@type\": \"Interface\",    \"displayName\": \"Phone\",    \"contents\": [        {            \"@type\": \"Component\",            \"name\": \"frontCamera\",            \"schema\": \"dtmi:com:example:Camera;3\"        },        {            \"@type\": \"Component\",            \"name\": \"backCamera\",            \"schema\": \"dtmi:com:example:Camera;3\"        },        {            \"@type\": \"Component\",            \"name\": \"deviceInfo\",            \"schema\": \"dtmi:azure:deviceManagement:DeviceInformation;2\"        }    ],    \"@context\": \"dtmi:dtdl:context;2\"}";

            var deserialized = JsonSerializer.Deserialize<Interface>(json, DtdlConverter.Options);

            Assert.AreEqual(deserialized.Id, "dtmi:com:example:Phone;2");
            Assert.AreEqual(deserialized.DisplayName, "Phone");
            Assert.AreEqual(deserialized.Type, DtdlTypeEnum.Interface);
            Assert.AreEqual(deserialized.Context, "dtmi:dtdl:context;2");
            Assert.IsNotNull(deserialized.Contents);
            Assert.IsNotNull(deserialized.Contents.Properties);
            Assert.IsNotNull(deserialized.Contents.Telemetry);
            Assert.IsNotNull(deserialized.Contents.Commands);
            Assert.IsNotNull(deserialized.Contents.Relationships);
            Assert.IsNotNull(deserialized.Contents.Components);
            Assert.AreEqual(deserialized.Contents.Properties.Count, 0);
            Assert.AreEqual(deserialized.Contents.Telemetry.Count, 0);
            Assert.AreEqual(deserialized.Contents.Relationships.Count, 0);
            Assert.AreEqual(deserialized.Contents.Components.Count, 3);
            Assert.AreEqual(deserialized.Contents.Components[0].Name, "frontCamera");
            Assert.AreEqual(deserialized.Contents.Components[0].Type, DtdlTypeEnum.Component);
            Assert.AreEqual(deserialized.Contents.Components[0].Schema.ReferenceId, "dtmi:com:example:Camera;3");
            Assert.IsTrue(deserialized.Contents.Components[0].Schema.IsReferenceId);
            Assert.IsNull(deserialized.Comment);
        }
    }
}