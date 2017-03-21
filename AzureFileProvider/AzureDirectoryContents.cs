using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.FileProviders;

namespace WebApplication2
{
    public class AzureDirectoryContents : IDirectoryContents
    {
        private readonly CloudBlobDirectory _blob;
        private readonly BlobResultSegment _directoryContent;

        public AzureDirectoryContents(CloudBlobDirectory blob)
        {
            _blob = blob;
            _directoryContent = blob.ListBlobsSegmentedAsync(null).Result;
            Exists = _directoryContent.Results != null && _directoryContent.Results.Any();
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return ContentToFileInfo().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ContentToFileInfo().GetEnumerator();
        }

        public bool Exists { get; }

        private IEnumerable<IFileInfo> ContentToFileInfo()
        {
            if (_directoryContent == null || _directoryContent.Results == null || !_directoryContent.Results.Any())
            {
                return new IFileInfo[0];
            }

            return _directoryContent.Results.Select(blob => new AzureFileInfo(blob));
        }
    }
}