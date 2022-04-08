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

        [TestMethod]
        public void Interface_Relationship_Test()
        {

            string json = "{    \"@id\": \"dtmi:com:example:Building;1\",    \"@type\": \"Interface\",    \"displayName\": \"Building\",    \"contents\": [        {            \"@type\": \"Property\",            \"name\": \"name\",            \"schema\": \"string\",            \"writable\": true        },        {            \"@type\": \"Relationship\",            \"name\": \"contains\",            \"target\": \"dtmi:com:example:Room;1\"        }    ],    \"@context\": \"dtmi:dtdl:context;2\"}";

            var deserialized = JsonSerializer.Deserialize<Interface>(json, DtdlConverter.Options);

            Assert.AreEqual(deserialized.Id, "dtmi:com:example:Building;1");
            Assert.AreEqual(deserialized.DisplayName, "Building");
            Assert.AreEqual(deserialized.Type, DtdlTypeEnum.Interface);
            Assert.AreEqual(deserialized.Context, "dtmi:dtdl:context;2");
            Assert.IsNotNull(deserialized.Contents);
            Assert.IsNotNull(deserialized.Contents.Properties);
            Assert.IsNotNull(deserialized.Contents.Telemetry);
            Assert.IsNotNull(deserialized.Contents.Commands);
            Assert.IsNotNull(deserialized.Contents.Relationships);
            Assert.IsNotNull(deserialized.Contents.Components);
            Assert.AreEqual(deserialized.Contents.Properties.Count, 1);
            Assert.AreEqual(deserialized.Contents.Relationships.Count, 1);
            Assert.AreEqual(deserialized.Contents.Telemetry.Count, 0);
            Assert.AreEqual(deserialized.Contents.Commands.Count, 0);
            Assert.AreEqual(deserialized.Contents.Components.Count, 0);
            Assert.AreEqual(deserialized.Contents.Properties[0].Name, "name");
            Assert.AreEqual(deserialized.Contents.Properties[0].Type.TypeName, DtdlTypeEnum.Property);
            Assert.IsNull(deserialized.Contents.Properties[0].Type.SemanticType);
            Assert.AreEqual(deserialized.Contents.Properties[0].Schema.Primitive, SchemaPrimitive.String);
            Assert.AreEqual(deserialized.Contents.Relationships[0].Name, "contains");
            Assert.AreEqual(deserialized.Contents.Relationships[0].Target, "dtmi:com:example:Room;1");
            Assert.IsNull(deserialized.Comment);
        }

        [TestMethod]
        public void DtdlModel_OneInterface_Test()
        {

            string json = "{    \"@id\": \"dtmi:com:example:Thermostat;1\",    \"@type\": \"Interface\",    \"displayName\": \"Thermostat\",    \"contents\": [        {            \"@type\": \"Telemetry\",            \"name\": \"temp\",            \"schema\": \"double\"        },        {            \"@type\": \"Property\",            \"name\": \"setPointTemp\",            \"writable\": true,            \"schema\": \"double\"        }    ],    \"@context\": \"dtmi:dtdl:context;2\"}";

            var deserialized = JsonSerializer.Deserialize<DtdlModel>(json, DtdlConverter.Options);

            Assert.IsNotNull(deserialized.Interfaces);
            Assert.AreEqual(deserialized.Interfaces.Count, 1);
            Assert.AreEqual(deserialized.Interfaces[0].Id, "dtmi:com:example:Thermostat;1");
            Assert.AreEqual(deserialized.Interfaces[0].DisplayName, "Thermostat");
            Assert.AreEqual(deserialized.Interfaces[0].Type, DtdlTypeEnum.Interface);
            Assert.AreEqual(deserialized.Interfaces[0].Context, "dtmi:dtdl:context;2");
            Assert.IsNotNull(deserialized.Interfaces[0].Contents);
            Assert.IsNotNull(deserialized.Interfaces[0].Contents.Properties);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Properties.Count, 1);
            Assert.IsTrue(deserialized.Interfaces[0].Contents.Properties[0].Writable);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Properties[0].Name, "setPointTemp");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Properties[0].Schema.Primitive, SchemaPrimitive.Double);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Properties[0].Type.TypeName, DtdlTypeEnum.Property);
            Assert.IsNull(deserialized.Interfaces[0].Contents.Properties[0].Type.SemanticType);
            Assert.IsNotNull(deserialized.Interfaces[0].Contents.Telemetry);
            Assert.IsNotNull(deserialized.Interfaces[0].Contents.Commands);
            Assert.IsNotNull(deserialized.Interfaces[0].Contents.Relationships);
            Assert.IsNotNull(deserialized.Interfaces[0].Contents.Components);
            Assert.IsNull(deserialized.Interfaces[0].Comment);
        }

        [TestMethod]
        public void DtdlModel_MultipleInterfaces_Test()
        {

            string json = "[    {        \"@id\": \"dtmi:com:example:Room;1\",        \"@type\": \"Interface\",        \"contents\": [            {                \"@type\": \"Property\",                \"name\": \"occupied\",                \"schema\": \"boolean\"            }        ],        \"@context\": \"dtmi:dtdl:context;2\"    },    {        \"@id\": \"dtmi:com:example:ConferenceRoom;1\",        \"@type\": \"Interface\",        \"extends\": \"dtmi:com:example:Room;1\",        \"contents\": [            {                \"@type\": \"Property\",                \"name\": \"capacity\",                \"schema\": \"integer\"            }        ],        \"@context\": \"dtmi:dtdl:context;2\"    }]";

            var deserialized = JsonSerializer.Deserialize<DtdlModel>(json, DtdlConverter.Options);

            Assert.IsNotNull(deserialized.Interfaces);
            Assert.AreEqual(deserialized.Interfaces.Count, 2);

            Assert.AreEqual(deserialized.Interfaces[0].Id, "dtmi:com:example:Room;1");
            Assert.AreEqual(deserialized.Interfaces[0].Type, DtdlTypeEnum.Interface);
            Assert.AreEqual(deserialized.Interfaces[0].Context, "dtmi:dtdl:context;2");
            Assert.IsNotNull(deserialized.Interfaces[0].Contents);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Properties.Count, 1);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Telemetry.Count, 0);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Relationships.Count, 0);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Commands.Count, 0);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Components.Count, 0);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Properties[0].Name, "occupied");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Properties[0].Type.TypeName, DtdlTypeEnum.Property);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Properties[0].Schema.Primitive, SchemaPrimitive.Boolean);
            Assert.IsNull(deserialized.Interfaces[0].Comment);

            Assert.AreEqual(deserialized.Interfaces[1].Id, "dtmi:com:example:ConferenceRoom;1");
            Assert.AreEqual(deserialized.Interfaces[1].Type, DtdlTypeEnum.Interface);
            Assert.IsNotNull(deserialized.Interfaces[1].Extends);
            Assert.AreEqual(deserialized.Interfaces[1].Extends[0], "dtmi:com:example:Room;1");
            Assert.AreEqual(deserialized.Interfaces[1].Context, "dtmi:dtdl:context;2");
            Assert.IsNotNull(deserialized.Interfaces[1].Contents);
            Assert.AreEqual(deserialized.Interfaces[1].Contents.Properties.Count, 1);
            Assert.AreEqual(deserialized.Interfaces[1].Contents.Telemetry.Count, 0);
            Assert.AreEqual(deserialized.Interfaces[1].Contents.Relationships.Count, 0);
            Assert.AreEqual(deserialized.Interfaces[1].Contents.Commands.Count, 0);
            Assert.AreEqual(deserialized.Interfaces[1].Contents.Components.Count, 0);
            Assert.AreEqual(deserialized.Interfaces[1].Contents.Properties[0].Name, "capacity");
            Assert.AreEqual(deserialized.Interfaces[1].Contents.Properties[0].Type.TypeName, DtdlTypeEnum.Property);
            Assert.AreEqual(deserialized.Interfaces[1].Contents.Properties[0].Schema.Primitive, SchemaPrimitive.Integer);
            Assert.IsNull(deserialized.Interfaces[1].Comment);

        }
    }
}