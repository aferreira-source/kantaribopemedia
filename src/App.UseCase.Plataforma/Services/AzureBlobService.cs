using app.plataforma.Identity;
using app.plataforma.Interfaces;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Identity;

namespace app.plataforma.Services;

public class AzureBlobService : IAzureBlobService
{
    private readonly MongoDBContext _context;
    private UserManager<ApplicationUser> _userManager;
    //protected readonly BlobContainerClient _blobServiceClient;
    protected readonly AzureStorage _azureStorage;

    private const string ContainerName = "portal-blob";
    public const string SuccessMessageKey = "SuccessMessage";
    public const string ErrorMessageKey = "ErrorMessage";
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _containerClient;

    public AzureBlobService(MongoDBContext context, UserManager<ApplicationUser> userManager,
        AzureStorage azureStorage)
    {
        _azureStorage = azureStorage;

        _containerClient = new BlobContainerClient(
            new Uri($"{_azureStorage.Url}/{_azureStorage.BlobName}"),
            new StorageSharedKeyCredential(_azureStorage.AccountName, _azureStorage.Signature)
          );
        _containerClient.CreateIfNotExists();


        _blobServiceClient = new BlobServiceClient(
            new Uri($"{_azureStorage.Url}/{_azureStorage.BlobName}"),
            new StorageSharedKeyCredential(_azureStorage.AccountName, _azureStorage.Signature)
          );

        _context = context;
        _userManager = userManager;
    }

    public async Task<BlobContentInfo> UploadFile(byte[] blob, string filename)
    {

        using (Stream str = new MemoryStream(blob))
        {
            string blobName = filename;
            var blobClient = _containerClient.GetBlobClient(blobName);
            var uploadOptions = new BlobUploadOptions() { HttpHeaders = new BlobHttpHeaders() { ContentType = "video/mp4" } };
            return await blobClient.UploadAsync(str, uploadOptions);
        }
    }


    public async Task<BlobContentInfo> GetPathFile(string filename)
    {
        return null;
    }
}