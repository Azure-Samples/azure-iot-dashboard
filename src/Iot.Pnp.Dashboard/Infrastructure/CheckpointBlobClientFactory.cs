using Azure.Identity;
using Azure.Storage.Blobs;
using Iot.PnpDashboard.Configuration;

namespace Iot.PnpDashboard.Infrastructure
{
    public class CheckpointBlobClientFactory
    {
        private readonly AppConfiguration _configuration;

        public CheckpointBlobClientFactory(AppConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<BlobContainerClient> CreateAsync()
        {
            if (_configuration.ManagedIdentityEnabled)
            {
                return await CreateContainerClientWithMSI();
            }

            return await CreateContainerClientFromConnStr();
        }

        private async Task<BlobContainerClient> CreateContainerClientWithMSI()
        {
            DefaultAzureCredentialOptions options = new DefaultAzureCredentialOptions()
            {
                ManagedIdentityClientId = _configuration.ManagedIdentityClientId ?? null
            };
            var credential = new DefaultAzureCredential(options);
            
            var storageUrl = $"https://{_configuration.CheckpointStaAccountName}.blob.core.windows.net/{_configuration.CheckpointStaContainer}";

            var token = credential.GetToken(new Azure.Core.TokenRequestContext(new string[] { "https://management.azure.com/.default" }));

            var checkpointStore = new BlobContainerClient(new Uri(storageUrl), credential);
            await checkpointStore.CreateIfNotExistsAsync();
            return checkpointStore;
        }

        private async Task<BlobContainerClient> CreateContainerClientFromConnStr()
        {
            var checkpointStore = new BlobContainerClient(
                _configuration.CheckpointStaConnString, 
                _configuration.CheckpointStaContainer);
            await checkpointStore.CreateIfNotExistsAsync();
            return checkpointStore;
        }
    }
}
