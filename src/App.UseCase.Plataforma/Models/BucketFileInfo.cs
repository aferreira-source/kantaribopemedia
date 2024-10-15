using MongoDB.Bson;

namespace app.plataforma;

public class BucketFileInfo
{
    public ObjectId Id { get; set; }

    public DateTime UploadTime { get; private set; }

    public string FileName { get; set; }

    public long FileSize { get; set; }

    public BucketFileInfo()
    {
        Id = ObjectId.GenerateNewId();
        UploadTime = DateTime.Now;
    }
}



