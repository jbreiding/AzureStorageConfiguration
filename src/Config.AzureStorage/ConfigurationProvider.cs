namespace Microsoft.Extensions.Configuration.AzureStorage
{
    using System;
    using Newtonsoft.Json;
    using System.IO;
    using System.IO.Compression;
    using WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class AzureStorageConfigurationProvider : ConfigurationProvider
    {
        AzureStorageConfigurationSource Source { get; set; }

        public AzureStorageConfigurationProvider(AzureStorageConfigurationSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public override void Load()
        {
            try
            {
                var blob = Source.StorageAccount.CreateCloudBlobClient().
                GetContainerReference(Source.ContainerName).
                GetBlobReference(Source.BlobName);

                if (!blob.ExistsAsync().Result && Source.Optional)
                {
                    return;
                }
                else if (!blob.ExistsAsync().Result)
                {
                    throw new FileNotFoundException($"{blob.StorageUri}");
                }

                if (!blob.Properties.ContentType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new FormatException($"{blob.Properties.ContentType} is not application/json");
                }

                Data = JsonConfigurationStreamParser.Parse(GetStream(blob));
            }
            catch (JsonReaderException e)
            {
                throw new FormatException($"Could not parse the JSON blob. Error on line number '{e.LineNumber}'.", e);
            }
        }

        private Stream GetStream(CloudBlob blob)
        {
            if (blob.Properties.ContentEncoding?.Equals("gzip", StringComparison.InvariantCultureIgnoreCase) ?? false)
            {
                using (var bigStreamOut = new MemoryStream())
                using (var inStream = new MemoryStream())
                {
                    blob.DownloadToStreamAsync(inStream).Wait();
                    inStream.Seek(0, SeekOrigin.Begin);

                    using (var bigStream = new GZipStream(inStream, CompressionMode.Decompress))
                    {
                        bigStream.CopyTo(bigStreamOut);
                    }

                    return new MemoryStream(bigStreamOut.ToArray());
                }
            }
            else
            {
                var inStream = new MemoryStream();
                blob.DownloadToStreamAsync(inStream).Wait();
                return new MemoryStream(inStream.ToArray());
            }
        }
    }
}
