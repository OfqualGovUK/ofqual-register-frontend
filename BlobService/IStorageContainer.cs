using Azure.Storage.Blobs.Models;

namespace Ofqual.Common.RegisterFrontend.BlobStorage
{
    public interface IBlobService
    {
        Task UploadBlob(string blobName, Stream blobContent);
        bool BlobExists(string blobname);
        Task<BlobProperties> BlobProperties(string blobname);

        Task<MemoryStream> DownloadBlob(string blobname);
    }
}
