using app.plataforma.Identity;
using app.plataforma.Interfaces;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace app.plataforma.Services;

public class BlobService : IBlobService
{
    private readonly MongoDBContext _context;
    private UserManager<ApplicationUser> _userManager;
    protected readonly BlobContainerClient _blobServiceClient;
    protected readonly AzureStorage _azureStorage;

    public BlobService(MongoDBContext context, UserManager<ApplicationUser> userManager, AzureStorage azureStorage)
    {
        _azureStorage = azureStorage;


        AzureSasCredential credential = new AzureSasCredential(_azureStorage.Chave);
        var url = new Uri(_azureStorage.Url + @"\" + _azureStorage.BlobName);
        _blobServiceClient = new BlobContainerClient(url, credential);

        _context = context;
        _userManager = userManager;
    }

    public async Task<bool> UploadFile(string blob, string filename)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(blob);
        Stream str = new MemoryStream(bytes);
        Response<BlobContentInfo> blobResult = null;
        try
        {
            blobResult = await _blobServiceClient.UploadBlobAsync(filename, str);
        }
        catch (Exception exc)
        {
            var message = exc.Message;
        }

        var result = blobResult.GetRawResponse().Status;
        return true;
    }
}