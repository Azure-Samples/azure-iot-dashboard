using Microsoft.VisualStudio.TestTools.UnitTesting;
using Iot.PnpDashboard.Devices.Dtdl;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Azure;
using System.Text.Json;
using Microsoft.Azure.Amqp.Framing;

namespace Iot.PnpDashboard.Tests
{
    [TestClass]
    public class DeserializeRelationshipTests
    {
        [TestMethod]
        public void Relationship_Multiplicity_Test()
        {

            string json = "{    \"@type\": \"Relationship\",    \"name\": \"floor\",    \"minMultiplicity\": 0,    \"maxMultiplicity\": 1,    \"target\": \"dtmi:com:example:Floor;1\"}";

            var deserialized = JsonSerializer.Deserialize<Relationship>(json, DtdlConverter.Options);

            Assert.AreEqual(deserialized.Name, "floor");
            Assert.AreEqual(deserialized.Type, DtdlTypeEnum.Relationship);
            Assert.IsNull(deserialized.Id);
            Assert.IsNull(deserialized.Description);
            Assert.IsNull(deserialized.Comment);
            Assert.IsNull(deserialized.DisplayName);
            Assert.IsNull(deserialized.Properties);
            Assert.AreEqual(deserialized.Target, "dtmi:com:example:Floor;1");
            Assert.AreEqual(deserialized.MinMultiplicity, 0);
            Assert.AreEqual(deserialized.MaxMultiplicity, 1);
        }

        [TestMethod]
        public void Relationship_Simple_Test()
        {

            string json = "{    \"@type\": \"Relationship\",    \"name\": \"children\"}";

            var deserialized = JsonSerializer.Deserialize<Relationship>(json, DtdlConverter.Options);

            Assert.AreEqual(deserialized.Name, "children");
            Assert.AreEqual(deserialized.Type, DtdlTypeEnum.Relationship);
            Assert.IsNull(deserialized.Id);
            Assert.IsNull(deserialized.Description);
            Assert.IsNull(deserialized.Comment);
            Assert.IsNull(deserialized.DisplayName);
            Assert.IsNull(deserialized.Properties);
            Assert.IsNull(deserialized.Target);
            Assert.IsFalse(deserialized.MinMultiplicity.HasValue);
            Assert.IsFalse(deserialized.MaxMultiplicity.HasValue);
        }


        [TestMethod]
        public void Relationship_Properties_Test()
        {

            string json = "{    \"@type\": \"Relationship\",    \"name\": \"cleanedBy\",    \"target\": \"dtmi:com:example:Cleaner;1\",    \"properties\": [        {            \"@type\": \"Property\",            \"name\": \"lastCleaned\",            \"schema\": \"dateTime\"        }    ]}";

            var deserialized = JsonSerializer.Deserialize<Relationship>(json, DtdlConverter.Options);

            Assert.AreEqual(deserialized.Name, "cleanedBy");
            Assert.AreEqual(deserialized.Type, DtdlTypeEnum.Relationship);
            Assert.IsNull(deserialized.Id);
            Assert.IsNull(deserialized.Description);
            Assert.IsNull(deserialized.Comment);
            Assert.IsNull(deserialized.DisplayName);
            Assert.IsNotNull(deserialized.Properties);
            Assert.IsNotNull(deserialized.Target);
            Assert.AreEqual(deserialized.Target, "dtmi:com:example:Cleaner;1");
            Assert.IsFalse(deserialized.MinMultiplicity.HasValue);
            Assert.IsFalse(deserialized.MaxMultiplicity.HasValue);
            Assert.AreEqual(deserialized.Properties.Count, 1);
            Assert.AreEqual(deserialized.Properties[0].Type.TypeName, DtdlTypeEnum.Property);
            Assert.AreEqual(deserialized.Properties[0].Schema, SchemaPrimitive.DateTime);
            Assert.IsNull(deserialized.Properties[0].Type.SemanticType);

        }
    }
}