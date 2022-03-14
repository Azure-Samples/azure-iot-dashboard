using Azure.Identity;
using Iot.PnpDashboard.Configuration;
using Microsoft.Azure.Devices;

namespace Iot.PnpDashboard.Infrastructure
{
    public class RegistryManagerFactory
    {
        private readonly AppConfiguration _configuration;

        public RegistryManagerFactory(AppConfiguration configuration)
        {
            _configuration = configuration;
        }

        public RegistryManager Create()
        {
            if (_configuration.ManagedIdentityEnabled)
            {
                return CreateRegistryManagerWithMSI();
            }

            return CreateRegistryManagerConnStr();
        }

        private RegistryManager CreateRegistryManagerWithMSI()
        {
            DefaultAzureCredentialOptions options = new DefaultAzureCredentialOptions()
            {
                ManagedIdentityClientId = _configuration.ManagedIdentityClientId ?? null
            };
            var credential = new DefaultAzureCredential(options);
            return RegistryManager.Create(_configuration.IotHubHostName, credential);
        }

        private RegistryManager CreateRegistryManagerConnStr()
        {
            return RegistryManager.CreateFromConnectionString(_configuration.IotHubConnStr);
        }
    }
}
