using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.FileProviders;

namespace WebApplication2
{
    public class AzureFileProvider : IFileProvider
    {
        private readonly CloudBlobContainer _container;

        public AzureFileProvider(IConfigurationRoot configurationRoot)
        {
            var connectionString = "YOUR AZURE STORAGE CONNECTION STRING OR READ FROM CONFIG";
            var containerName = "CONTAINER WHICH STORE THE CHALLENGE FILES";

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(containerName);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var azurePath = ConvertPath(subpath);
            var blob = _container.GetBlockBlobReference(azurePath);
            return new AzureFileInfo(blob);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var azurePath = ConvertPath(subpath);
            var blob = _container.GetDirectoryReference(azurePath);
            return new AzureDirectoryContents(blob);
        }

        public IChangeToken Watch(string filter)
        {
            return new NoWatchChangeToken();
        }

        private string ConvertPath(string path)
        {
            if (path.StartsWith("/Views/", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring(7);
            }
            if (path.StartsWith("Views/", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring(6);
            }
            if (path.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring(1);
            }
            return path;
        }
    }
}
