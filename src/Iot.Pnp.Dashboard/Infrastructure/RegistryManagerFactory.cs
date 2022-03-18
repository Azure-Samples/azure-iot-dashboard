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
            return CreateRegistryManagerConnStr();
        }

        private RegistryManager CreateRegistryManagerConnStr()
        {
            return RegistryManager.CreateFromConnectionString(_configuration.IotHubConnStr);
        }
    }
}
