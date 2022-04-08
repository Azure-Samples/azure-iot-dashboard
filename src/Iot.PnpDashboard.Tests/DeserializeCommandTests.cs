using Microsoft.VisualStudio.TestTools.UnitTesting;
using Iot.PnpDashboard.Devices.Dtdl;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Azure;
using System.Text.Json;

namespace Iot.PnpDashboard.Tests
{
    [TestClass]
    public class DeserializeCommandTests
    {
        [TestMethod]
        public void Command_Test()
        {

            string json = "{    \"@type\": \"Command\",    \"name\": \"reboot\",    \"request\": {        \"name\": \"rebootTime\",        \"displayName\": \"Reboot Time\",        \"description\": \"Requested time to reboot the device.\",        \"schema\": \"dateTime\"    },    \"response\": {        \"name\": \"scheduledTime\",        \"schema\": \"dateTime\"    }}";

            var deserialized = JsonSerializer.Deserialize<Command>(json, DtdlConverter.Options);

            Assert.AreEqual(deserialized.Name, "reboot");
            Assert.AreEqual(deserialized.Type, DtdlTypeEnum.Command);
            Assert.IsNull(deserialized.Id);
            Assert.IsNull(deserialized.LocalizableDescription);
            Assert.IsNull(deserialized.Comment);
            Assert.IsNull(deserialized.LocalizableDisplayName);
            Assert.AreEqual(deserialized.Request.Name, "rebootTime");
            Assert.AreEqual(deserialized.Request.DisplayName, "Reboot Time");
            Assert.AreEqual(deserialized.Request.Description, "Requested time to reboot the device.");
            Assert.AreEqual(deserialized.Request.Schema.Primitive, SchemaPrimitive.DateTime);
            Assert.AreEqual(deserialized.Response.Name, "scheduledTime");
            Assert.AreEqual(deserialized.Response.Schema.Primitive, SchemaPrimitive.DateTime);
        }
    }
}