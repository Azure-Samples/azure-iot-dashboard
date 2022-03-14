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
            if (_configuration.ManagedIdentityEnabled)
            {
                return CreateDigitalTwinClientWithMSI();
            }

            return CreateDigitalTwinClientConnStr();
        }

        private DigitalTwinClient CreateDigitalTwinClientWithMSI()
        {
            DefaultAzureCredentialOptions options = new DefaultAzureCredentialOptions()
            {
                ManagedIdentityClientId = _configuration.ManagedIdentityClientId ?? null
            };
            var credential = new DefaultAzureCredential(options);
            return DigitalTwinClient.Create(_configuration.IotHubHostName, credential);
        }

        private DigitalTwinClient CreateDigitalTwinClientConnStr()
        {
            return DigitalTwinClient.CreateFromConnectionString(_configuration.IotHubConnStr);
        }
    }
}
