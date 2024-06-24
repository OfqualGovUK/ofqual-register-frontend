using Azure.Identity;
using Azure.Storage.Blobs;
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
                // Upload text to a new block blob.
                //byte[] byteArray = Encoding.ASCII.GetBytes(blobContents);

                //using (MemoryStream stream = new MemoryStream(byteArray))
                //{
                    await _blobContainerClient.UploadBlobAsync(blobName, blobContents);
                //}
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
