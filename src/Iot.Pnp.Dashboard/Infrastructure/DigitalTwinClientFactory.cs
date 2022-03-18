using Azure.Identity;
using Iot.PnpDashboard.Configuration;
using Microsoft.Azure.Devices;

namespace Iot.PnpDashboard.Infrastructure
{
    public class DigitalTwinClientFactory
    {
        private readonly AppConfiguration _configuration;

        public DigitalTwinClientFactory(AppConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DigitalTwinClient Create()
        {
            return CreateDigitalTwinClientConnStr();
        }

        private DigitalTwinClient CreateDigitalTwinClientConnStr()
        {
            return DigitalTwinClient.CreateFromConnectionString(_configuration.IotHubConnStr);
        }
    }
}
