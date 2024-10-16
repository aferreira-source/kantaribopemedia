using Azure.Storage.Blobs.Models;

namespace app.plataforma.Interfaces;

public interface IAzureBlobService
{
    Task<BlobContentInfo> UploadFile(byte[] blob, string filename);

}
