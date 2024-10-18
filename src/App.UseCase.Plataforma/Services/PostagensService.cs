using app.plataforma.Handlers;
using app.plataforma.Identity;
using app.plataforma.Interfaces;
using AspNetCore.Identity.Mongo.Mongo;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using Wangkanai.Detection.Models;
using Wangkanai.Detection.Services;

namespace app.plataforma.Services;

public class PostagensService : IPostagensService
{
    private readonly MongoDBContext _context;
    private UserManager<ApplicationUser> _userManager;
    //private readonly string bucket = "BUCKET-ARQUIVOS";
    //private readonly GridFSBucket fsBucket = null;
    private readonly IWebHostEnvironment _environment;
    private readonly IAzureBlobService _bloService;
    private readonly AzureStorage _azureStorage;

    private readonly IDetectionService _detectionService;
    public PostagensService(MongoDBContext context,
        UserManager<ApplicationUser> userManager,
        AzureStorage azureStorage,
        IWebHostEnvironment environment,
        IAzureBlobService bloService,
        IDetectionService detectionService)
    {
        _context = context;
        _userManager = userManager;
        _environment = environment;
        //fsBucket = new GridFSBucket(_context.Postagens.Database, new GridFSBucketOptions { BucketName = bucket });
        _azureStorage = azureStorage;
        _bloService = bloService;
        _detectionService = detectionService;
    }


    public async Task DeletarAsync(object Id)
    {
        var postagens = await _context.Postagens.Find(p => p.Id == Id).FirstOrDefaultAsync();

        if (postagens != null)
            await _context.Postagens.DeleteOneAsync(p => p.Id == Id);
    }

    public async Task<List<Postagens>> ObterTodosPorIdUsuarioAsync(object IdUsuario)
    {
        //return await _context.Postagens.Find(_ => true).ToListAsync();
        return await _context.Postagens.Find(p => p.usuarioid == IdUsuario).ToListAsync();
    }

    public async Task InserirAsync(Postagens obj)
    {
        obj.nomeArquivo = string.Concat(Guid.NewGuid().ToString(), ".mp4");
        obj.bucketFileInfo = await UploadFile(obj.fileBlob.ToString(), obj.nomeArquivo);
        await _context.Postagens.InsertOneAsync(obj);
    }

    public async Task AlterarAsync(Postagens obj)
    {
        var postagens = await _context.Postagens.Find(p => p.Id == obj.Id).FirstOrDefaultAsync();

        if (postagens != null)
            await _context.Postagens.ReplaceOneAsync(p => p.Id == obj.Id, obj);
    }

    public async Task DeletarTodosPorIdUsuarioAsync(object IdUsuario)
    {
        var filter = Builders<Postagens>.Filter.Where(x => x.usuarioid == IdUsuario);
        await _context.Postagens.DeleteManyAsync(filter);
    }

    public Task<IUsuarioPostagem> CarregarDadosPorIdUsuarioAsync(object IdUsuario)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Postagens>> ObterPostagens()
    {
        var filter = Builders<Postagens>.Filter.Where(x => (x.dtHora_Expiracao == null || x.dtHora_Expiracao <= DateTime.Now));
        var sort = Builders<Postagens>.Sort.Descending(x => x.dtHora_Publicacao);
        var lstPostagens = await _context.Postagens.Aggregate()
                    .Match(filter)
                    .SortByDescending(u => u.dtHora_Publicacao).ToListAsync();






        return lstPostagens.Select(x => { x.linkFile = string.Concat("/upload/", x.nomeArquivo); x.isMobile = _detectionService.Device.Type == Device.Mobile; return x; });
        //return lstPostagens.Select(x => { x.linkFile = $"{_azureStorage.Url}/{_azureStorage.BlobName}/{x.nomeArquivo}"; return x; });
    }




    public async Task<BucketFileInfo> UploadFile(string file, string savename = null)
    {

        string path = Path.Combine(_environment.WebRootPath, "upload", savename);

        string base64data = file.Replace("data:video/mp4;base64,", string.Empty);
        base64data = base64data.Replace("data:video/mp4;ogg;base64,", string.Empty);
        byte[] ret = Convert.FromBase64String(base64data);

        var rest = await _bloService.UploadFile(ret, savename);

        return new BucketFileInfo()
        {
            FileName = savename,
            FileSize = ret.Length
        };

        //var id = await fsBucket.UploadFromBytesAsync(savename, Zip(file), new GridFSUploadOptions() { DisableMD5 = false, ChunkSizeBytes = 261120 });

        //byte[] textBytes = Encoding.ASCII.GetBytes(file);
        //var id = await fsBucket.UploadFromBytesAsync(savename, ret, new GridFSUploadOptions() { });

        //var fileInfo = new BucketFileInfo(DateTime.UtcNow)
        //{
        //    Id = id,
        //    FileName = savename,
        //    FileSize = textBytes.Length,
        //};

        //_context.BucketFileInfo.InsertOne(fileInfo);

    }

    //public async Task<string> GetFile(ObjectId? id)
    //{
    //    var bucketFileInfo = _context.BucketFileInfo.Find(fileInfo => fileInfo.Id.Equals(id));
    //    var result = string.Empty;

    //    if (bucketFileInfo.Any())
    //        result = System.Text.Encoding.Default.GetString(fsBucket.DownloadAsBytes(id));
    //    return result;
    //}



}