using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Text;

namespace Ofqual.Common.RegisterFrontend.BlobStorage
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;

        public BlobService(BlobServiceClient blobServiceClient, IConfiguration configuration)
        {
            _blobServiceClient = blobServiceClient;

            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(configuration.GetSection("ContainerName").Value);
        }

        public async Task UploadBlob(string blobName, Stream blobContents)
        {
            try
            {
                await _blobContainerClient.UploadBlobAsync(blobName, blobContents);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool BlobExists(string blobname)
        {
            try
            {
                var blobClient = _blobContainerClient.GetBlobClient(blobname);

                return blobClient.Exists();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<BlobProperties> BlobProperties(string blobname)
        {
            try
            {
                var blobClient = _blobContainerClient.GetBlobClient(blobname);

                return await blobClient.GetPropertiesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<MemoryStream> DownloadBlob(string blobname)
        {
            try
            {
                var blobClient = _blobContainerClient.GetBlobClient(blobname);

                var stream = await blobClient.OpenReadAsync();

                MemoryStream memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                return memoryStream;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
