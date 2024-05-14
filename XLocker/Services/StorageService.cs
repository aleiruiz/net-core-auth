using Azure.Storage.Blobs;

namespace XLocker.Services
{

    public interface IStorageService
    {
        Task<string> UploadFileFromByteArray(byte[] img, string name);
        Task<string> UploadFileFromMemoryStream(MemoryStream file, string name);
    }
    public class StorageService : IStorageService
    {
        public BlobContainerClient containerClient;
        public StorageService(IConfiguration configuration)
        {
            containerClient = new BlobContainerClient(configuration.GetConnectionString("AzureBlobStorage"), configuration["ContainerName"]);
        }

        public async Task<string> UploadFileFromByteArray(byte[] img, string name)
        {
            BlobClient blobClient = containerClient.GetBlobClient(name);

            var ms = new MemoryStream(img, false);

            await blobClient.UploadAsync(ms);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> UploadFileFromMemoryStream(MemoryStream file, string name)
        {
            BlobClient blobClient = containerClient.GetBlobClient(name);
            file.Position = 0;
            await blobClient.UploadAsync(file);

            return blobClient.Uri.AbsoluteUri;
        }
    }
}
