namespace Ofqual.Common.RegisterFrontend.BlobStorage
{
    public interface IBlobService
    {
        Task UploadBlob(string blobName, Stream blobContent);
    }
}
