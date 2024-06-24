namespace Ofqual.Common.RegisterFrontend.BlobStorage
{
    public interface IBlobService
    {
        Task UploadBlob(string blobName, string blobContent);
    }
}
