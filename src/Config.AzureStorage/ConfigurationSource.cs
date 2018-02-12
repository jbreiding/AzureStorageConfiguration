namespace Microsoft.Extensions.Configuration.AzureStorage
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.WindowsAzure.Storage;

    public class AzureStorageConfigurationSource : IConfigurationSource
    {
        public CloudStorageAccount StorageAccount { get; set; }
        public string ContainerName { get; set; }
        public string BlobName { get; set; }
        public bool Optional { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureStorageConfigurationProvider(this);
        }
    }
}
