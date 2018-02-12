namespace Microsoft.Extensions.Configuration.AzureStorage.Test
{
    using System;
    using System.Globalization;
    using System.IO;
    using Xunit;
    using WindowsAzure.Storage;
    using Microsoft.Extensions.Configuration;

    public class AzureStorageTest
    {
        string connectionString;

        public AzureStorageTest()
        {
            var config = new ConfigurationBuilder().AddUserSecrets<AzureStorageTest>().Build();

            connectionString = config["connectionString"];
        }

        private IConfigurationRoot LoadConfig(string blobName)
        {
            var config = new ConfigurationBuilder()
            .AddAzureBlobStorage(connectionString, "configtest", blobName)
            .Build();

            return config;
        }

        [Fact]
        public void LoadKeyValuePairsFromValidJson()
        {
            var jsonConfigSrc = LoadConfig("valid.json");

            Assert.Equal("test", jsonConfigSrc["firstname"]);
            Assert.Equal("last.name", jsonConfigSrc["test.last.name"]);
            Assert.Equal("Something street", jsonConfigSrc["residential.address:STREET.name"]);
            Assert.Equal("12345", jsonConfigSrc["residential.address:zipcode"]);
        }

        [Fact]
        public void LoadMethodCanHandleEmptyValue()
        {
            var jsonConfigSrc = LoadConfig("emptyvalue.json");
            Assert.Equal(string.Empty, jsonConfigSrc["name"]);
        }

        [Fact]
        public void LoadWithCulture()
        {
            var previousCulture = CultureInfo.CurrentCulture;

            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("fr-FR");

                var jsonConfigSrc = LoadConfig("withculure.json");
                Assert.Equal("3.14", jsonConfigSrc["number"]);
            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
            }
        }

        [Fact]
        public void NonObjectRootIsInvalid()
        {
            var exception = Assert.Throws<FormatException>(
                () => LoadConfig("test.json"));

            Assert.NotNull(exception.Message);
        }

        [Fact]
        public void SupportAndIgnoreComments()
        {
            var jsonConfigSrc = LoadConfig("comments.json");
            Assert.Equal("test", jsonConfigSrc["name"]);
            Assert.Equal("Something street", jsonConfigSrc["address:street"]);
            Assert.Equal("12345", jsonConfigSrc["address:zipcode"]);
        }

        [Fact]
        public void ThrowExceptionWhenUnexpectedEndFoundBeforeFinishParsing()
        {
            var exception = Assert.Throws<FormatException>(() => LoadConfig("invalid.json"));
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public void ThrowExceptionWhenMissingCurlyBeforeFinishParsing()
        {
            var exception = Assert.Throws<FormatException>(() => LoadConfig("invalid2.json"));
            Assert.Contains("Could not parse the JSON blob.", exception.Message);
        }

        [Fact]
        public void Throws_On_Missing_Configuration_File()
        {
            var config = new ConfigurationBuilder().AddAzureBlobStorage(connectionString, "missingcontainer", "NotExistingConfig.json", optional: false);
            var exception = Assert.Throws<FileNotFoundException>(() => config.Build());

            // Assert
            Assert.Contains("NotExistingConfig.json", exception.Message);
        }

        [Fact]
        public void Does_Not_Throw_On_Optional_Configuration()
        {
            var config = new ConfigurationBuilder().AddAzureBlobStorage(connectionString, "missingcontainer", "missingblob", optional: true).Build();
        }

        [Fact]
        public void ThrowFormatExceptionWhenFileIsEmpty()
        {
            var exception = Assert.Throws<FormatException>(() => LoadConfig("empty.json"));
        }

        [Fact]
        public void LoadKeyValuePairsFromValidGzippedJson()
        {
            var jsonConfigSrc = new ConfigurationBuilder().AddAzureBlobStorage(connectionString, "gzipconfigtest", "valid.json.gz").Build();

            Assert.Equal("test", jsonConfigSrc["firstname"]);
            Assert.Equal("last.name", jsonConfigSrc["test.last.name"]);
            Assert.Equal("Something street", jsonConfigSrc["residential.address:STREET.name"]);
        }
    }
}
