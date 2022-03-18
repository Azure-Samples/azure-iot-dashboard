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
            return await CreateContainerClientFromConnStr();
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
