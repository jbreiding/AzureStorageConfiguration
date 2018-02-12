# AzureStorageConfiguration

This project supports pulling configuration from either vanilla json blob or gzipped json blob stored in an Azure Storage Blob container.

## Background

This began as a project which could deliver configuration to deployed services via Azure Storage.

## Installing AzureStorageConfiguration

//TODO

## Usage

Pull in the azure storage blob into your config.

```cs
    var config = new ConfigurationBuilder()
                  .AddAzureBlobStorage(connectionString, $"{containerName}", $"{blobName}")
                  .Build();
```

Access your config where you need them using:

```cs
    var value = config["key1"];
```

## Maintenance and Contributions

Contributions are always welcome.

## License and Acknowledgements 

Most of this code is directly taken from Microsoft's ASP.NET [Configuration](https://github.com/aspnet/Configuration) repo. As a result, this repo is under the [Apache License, Version 2.0](LICENSE.txt).
