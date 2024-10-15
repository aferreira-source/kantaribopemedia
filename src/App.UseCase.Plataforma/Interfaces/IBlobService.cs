using Azure.Storage.Blobs.Models;

namespace app.plataforma.Interfaces;

public interface IBlobService
{
    Task<BlobContentInfo> UploadFile(byte[] blob, string filename);

}
