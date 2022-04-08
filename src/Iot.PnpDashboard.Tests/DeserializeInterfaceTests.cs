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
            Assert.AreEqual(deserialized.Context[0], "dtmi:dtdl:context;2");
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
            Assert.AreEqual(deserialized.Context[0], "dtmi:dtdl:context;2");
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
            Assert.AreEqual(deserialized.Context[0], "dtmi:dtdl:context;2");
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
            Assert.AreEqual(deserialized.Interfaces[0].Context[0], "dtmi:dtdl:context;2");
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
            Assert.AreEqual(deserialized.Interfaces[0].Context[0], "dtmi:dtdl:context;2");
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
            Assert.AreEqual(deserialized.Interfaces[1].Context[0], "dtmi:dtdl:context;2");
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

        [TestMethod]
        public void DtdlModel_PnPDevice_Test()
        {

            string json = "[{  \"@context\": [    \"dtmi:iotcentral:context;2\",    \"dtmi:dtdl:context;2\"  ],  \"@id\": \"dtmi:com:example:TemperatureController;2\",  \"@type\": \"Interface\",  \"contents\": [    {      \"@type\": [        \"Telemetry\",        \"DataSize\"      ],      \"description\": {        \"en\": \"Current working set of the device memory in KiB.\"      },      \"displayName\": {        \"en\": \"Working Set\"      },      \"name\": \"workingSet\",      \"schema\": \"double\",      \"unit\": \"kibibit\"    },    {      \"@type\": \"Property\",      \"displayName\": {        \"en\": \"Serial Number\"      },      \"name\": \"serialNumber\",      \"schema\": \"string\",      \"writable\": false    },    {      \"@type\": \"Command\",      \"commandType\": \"synchronous\",      \"description\": {        \"en\": \"Reboots the device after waiting the number of seconds specified.\"      },      \"displayName\": {        \"en\": \"Reboot\"      },      \"name\": \"reboot\",      \"request\": {        \"@type\": \"CommandPayload\",        \"description\": {          \"en\": \"Number of seconds to wait before rebooting the device.\"        },        \"displayName\": {          \"en\": \"Delay\"        },        \"name\": \"delay\",        \"schema\": \"integer\"      }    },    {      \"@type\": \"Component\",      \"displayName\": {        \"en\": \"thermostat1\"      },      \"name\": \"thermostat1\",      \"schema\": \"dtmi:com:example:Thermostat;1\"    },    {      \"@type\": \"Component\",      \"displayName\": {        \"en\": \"thermostat2\"      },      \"name\": \"thermostat2\",      \"schema\": \"dtmi:com:example:Thermostat;2\"    },    {      \"@type\": \"Component\",      \"displayName\": {        \"en\": \"DeviceInfo\"      },      \"name\": \"deviceInformation\",      \"schema\": \"dtmi:azure:DeviceManagement:DeviceInformation;1\"    }  ],  \"displayName\": {    \"en\": \"Temperature Controller\"  }},{  \"@context\": \"dtmi:dtdl:context;2\",  \"@id\": \"dtmi:com:example:Thermostat;1\",  \"@type\": \"Interface\",  \"displayName\": \"Thermostat\",  \"description\": \"Reports current temperature and provides desired temperature control.\",  \"contents\": [    {      \"@type\": [        \"Telemetry\",        \"Temperature\"      ],      \"name\": \"temperature\",      \"displayName\": \"Temperature\",      \"description\": \"Temperature in degrees Celsius.\",      \"schema\": \"double\",      \"unit\": \"degreeCelsius\"    },    {      \"@type\": [        \"Property\",        \"Temperature\"      ],      \"name\": \"targetTemperature\",      \"schema\": \"double\",      \"displayName\": \"Target Temperature\",      \"description\": \"Allows to remotely specify the desired target temperature.\",      \"unit\": \"degreeCelsius\",      \"writable\": true    },    {      \"@type\": [        \"Property\",        \"Temperature\"      ],      \"name\": \"maxTempSinceLastReboot\",      \"schema\": \"double\",      \"unit\": \"degreeCelsius\",      \"displayName\": \"Max temperature since last reboot.\",      \"description\": \"Returns the max temperature since last device reboot.\"    },    {      \"@type\": \"Command\",      \"name\": \"getMaxMinReport\",      \"displayName\": \"Get Max-Min report.\",      \"description\": \"This command returns the max, min and average temperature from the specified time to the current time.\",      \"request\": {        \"name\": \"since\",        \"displayName\": \"Since\",        \"description\": \"Period to return the max-min report.\",        \"schema\": \"dateTime\"      },      \"response\": {        \"name\": \"tempReport\",        \"displayName\": \"Temperature Report\",        \"schema\": {          \"@type\": \"Object\",          \"fields\": [            {              \"name\": \"maxTemp\",              \"displayName\": \"Max temperature\",              \"schema\": \"double\"            },            {              \"name\": \"minTemp\",              \"displayName\": \"Min temperature\",              \"schema\": \"double\"            },            {              \"name\": \"avgTemp\",              \"displayName\": \"Average Temperature\",              \"schema\": \"double\"            },            {              \"name\": \"startTime\",              \"displayName\": \"Start Time\",              \"schema\": \"dateTime\"            },            {              \"name\": \"endTime\",              \"displayName\": \"End Time\",              \"schema\": \"dateTime\"            }          ]        }      }    }  ]},{  \"@context\": \"dtmi:dtdl:context;2\",  \"@id\": \"dtmi:com:example:Thermostat;2\",  \"@type\": \"Interface\",  \"displayName\": \"Thermostat\",  \"description\": \"Reports current temperature and provides desired temperature control.\",  \"contents\": [    {      \"@type\": [        \"Telemetry\",        \"Temperature\"      ],      \"name\": \"temperature\",      \"displayName\": \"Temperature\",      \"description\": \"Temperature in degrees Celsius.\",      \"schema\": \"double\",      \"unit\": \"degreeCelsius\"    },    {      \"@type\": [        \"Property\",        \"Temperature\"      ],      \"name\": \"targetTemperature\",      \"schema\": \"double\",      \"displayName\": \"Target Temperature\",      \"description\": \"Allows to remotely specify the desired target temperature.\",      \"unit\": \"degreeCelsius\",      \"writable\": true    },    {      \"@type\": [        \"Property\",        \"Temperature\"      ],      \"name\": \"maxTempSinceLastReboot\",      \"schema\": \"double\",      \"unit\": \"degreeCelsius\",      \"displayName\": \"Max temperature since last reboot.\",      \"description\": \"Returns the max temperature since last device reboot.\"    },    {      \"@type\": \"Command\",      \"name\": \"getMaxMinReport\",      \"displayName\": \"Get Max-Min report.\",      \"description\": \"This command returns the max, min and average temperature from the specified time to the current time.\",      \"request\": {        \"name\": \"since\",        \"displayName\": \"Since\",        \"description\": \"Period to return the max-min report.\",        \"schema\": \"dateTime\"      },      \"response\": {        \"name\": \"tempReport\",        \"displayName\": \"Temperature Report\",        \"schema\": {          \"@type\": \"Object\",          \"fields\": [            {              \"name\": \"maxTemp\",              \"displayName\": \"Max temperature\",              \"schema\": \"double\"            },            {              \"name\": \"minTemp\",              \"displayName\": \"Min temperature\",              \"schema\": \"double\"            },            {              \"name\": \"avgTemp\",              \"displayName\": \"Average Temperature\",              \"schema\": \"double\"            },            {              \"name\": \"startTime\",              \"displayName\": \"Start Time\",              \"schema\": \"dateTime\"            },            {              \"name\": \"endTime\",              \"displayName\": \"End Time\",              \"schema\": \"dateTime\"            }          ]        }      }    }  ]},{  \"@context\": \"dtmi:dtdl:context;2\",  \"@id\": \"dtmi:azure:DeviceManagement:DeviceInformation;1\",  \"@type\": \"Interface\",  \"displayName\": \"Device Information\",  \"contents\": [    {      \"@type\": \"Property\",      \"name\": \"manufacturer\",      \"displayName\": \"Manufacturer\",      \"schema\": \"string\",      \"description\": \"Company name of the device manufacturer. This could be the same as the name of the original equipment manufacturer (OEM). Ex. Contoso.\"    },    {      \"@type\": \"Property\",      \"name\": \"model\",      \"displayName\": \"Device model\",      \"schema\": \"string\",      \"description\": \"Device model name or ID. Ex. Surface Book 2.\"    },    {      \"@type\": \"Property\",      \"name\": \"swVersion\",      \"displayName\": \"Software version\",      \"schema\": \"string\",      \"description\": \"Version of the software on your device. This could be the version of your firmware. Ex. 1.3.45\"    },    {      \"@type\": \"Property\",      \"name\": \"osName\",      \"displayName\": \"Operating system name\",      \"schema\": \"string\",      \"description\": \"Name of the operating system on the device. Ex. Windows 10 IoT Core.\"    },    {      \"@type\": \"Property\",      \"name\": \"processorArchitecture\",      \"displayName\": \"Processor architecture\",      \"schema\": \"string\",      \"description\": \"Architecture of the processor on the device. Ex. x64 or ARM.\"    },    {      \"@type\": \"Property\",      \"name\": \"processorManufacturer\",      \"displayName\": \"Processor manufacturer\",      \"schema\": \"string\",      \"description\": \"Name of the manufacturer of the processor on the device. Ex. Intel.\"    },    {      \"@type\": \"Property\",      \"name\": \"totalStorage\",      \"displayName\": \"Total storage\",      \"schema\": \"double\",      \"description\": \"Total available storage on the device in kilobytes. Ex. 2048000 kilobytes.\"    },    {      \"@type\": \"Property\",      \"name\": \"totalMemory\",      \"displayName\": \"Total memory\",      \"schema\": \"double\",      \"description\": \"Total available memory on the device in kilobytes. Ex. 256000 kilobytes.\"    }  ]}]";

            var deserialized = JsonSerializer.Deserialize<DtdlModel>(json, DtdlConverter.Options);

            Assert.IsNotNull(deserialized.Interfaces);
            Assert.AreEqual(deserialized.Interfaces.Count, 4);

            Assert.AreEqual(deserialized.Interfaces[0].Context[0], "dtmi:iotcentral:context;2");
            Assert.AreEqual(deserialized.Interfaces[0].Context[1], "dtmi:dtdl:context;2");
            Assert.AreEqual(deserialized.Interfaces[0].Id, "dtmi:com:example:TemperatureController;2");
            Assert.AreEqual(deserialized.Interfaces[0].DisplayName, "Temperature Controller");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Telemetry[0].Type.TypeName, DtdlTypeEnum.Telemetry);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Telemetry[0].Type.SemanticType, "DataSize");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Telemetry[0].Description, "Current working set of the device memory in KiB.");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Telemetry[0].DisplayName, "Working Set");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Telemetry[0].Name, "workingSet");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Telemetry[0].Schema.Primitive, SchemaPrimitive.Double);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Telemetry[0].Unit, "kibibit");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Properties[0].DisplayName, "Serial Number");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Properties[0].Name, "serialNumber");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Properties[0].Schema.Primitive, SchemaPrimitive.String);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Properties[0].Writable, false);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Properties[0].Writable, false);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Commands[0].Type, DtdlTypeEnum.Command);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Commands[0].Description, "Reboots the device after waiting the number of seconds specified.");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Commands[0].DisplayName, "Reboot");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Commands[0].Name, "reboot");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Commands[0].Request.Description, "Number of seconds to wait before rebooting the device.");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Commands[0].Request.DisplayName, "Delay");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Commands[0].Request.Schema.Primitive, SchemaPrimitive.Integer);
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Commands[0].Request.DisplayName, "Delay");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Components[1].DisplayName, "thermostat2");
            Assert.AreEqual(deserialized.Interfaces[0].Contents.Components[1].Schema.ReferenceId, "dtmi:com:example:Thermostat;2");




        }
    }
}