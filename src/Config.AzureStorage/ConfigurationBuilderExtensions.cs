namespace Microsoft.Extensions.Configuration
{
    using Microsoft.Extensions.Configuration.AzureStorage;
    using System;
    using Microsoft.WindowsAzure.Storage;

    public static class AzureStorageExtensions
    {
        public static IConfigurationBuilder AddAzureBlobStorage(
            this IConfigurationBuilder configurationBuilder,
            string connectionString,
            string containerName,
            string blobName,
            bool optional = false
        ) => configurationBuilder.Add(new AzureStorageConfigurationSource(){
            StorageAccount = CloudStorageAccount.Parse(connectionString),
            ContainerName = containerName,
            BlobName = blobName,
            Optional = optional
        });
    }
}
