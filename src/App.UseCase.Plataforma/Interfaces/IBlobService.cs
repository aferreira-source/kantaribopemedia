namespace app.plataforma.Interfaces;

public interface IBlobService
{
    Task<bool> UploadFile(string blob, string filename);

}
