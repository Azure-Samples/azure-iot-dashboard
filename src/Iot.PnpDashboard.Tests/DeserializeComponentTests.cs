using Microsoft.VisualStudio.TestTools.UnitTesting;
using Iot.PnpDashboard.Devices.Dtdl;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Azure;
using System.Text.Json;

namespace Iot.PnpDashboard.Tests
{
    [TestClass]
    public class DeserializeComponentTests
    {
        [TestMethod]
        public void Component_Test()
        {

            string json = "{    \"@type\": \"Component\",    \"name\": \"frontCamera\",    \"schema\": \"dtmi:com:example:Camera;1\"}";

            var deserialized = JsonSerializer.Deserialize<Component>(json, DtdlConverter.Options);

            Assert.AreEqual(deserialized.Name, "frontCamera");
            Assert.AreEqual(deserialized.Type, DtdlTypeEnum.Component);
            Assert.IsNull(deserialized.Id);
            Assert.IsNull(deserialized.LocalizableDescription);
            Assert.IsNull(deserialized.Comment);
            Assert.IsNull(deserialized.LocalizableDisplayName);
            Assert.AreEqual(deserialized.Schema.ReferenceId, "dtmi:com:example:Camera;1");
        }
    }
}