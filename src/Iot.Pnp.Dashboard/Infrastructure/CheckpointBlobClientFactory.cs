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

        public async Task<BlobContainerClient> CreateContainerClientAsync()
        {
            if (_configuration.ManagedIdentityEnabled)
            {
                return await CreateContainerClientWithMSI();
            }

            return await CreateContainerClientFromConnStr();
        }

        private async Task<BlobContainerClient> CreateContainerClientWithMSI()
        {
            // When deployed to an azure host, the default azure credential will authenticate the specified user assigned managed identity.
            var credential = new DefaultAzureCredential();

            // var storageUrl = "https://iotplaygroundsta.blob.core.windows.net/iot-hub-checkpointing";
            var storageUrl = $"https://{_configuration.CheckpointStaAccountName}.blob.core.windows.net/{_configuration.CheckpointStaContainer}";

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
