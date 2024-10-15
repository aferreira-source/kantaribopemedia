using app.plataforma.Handlers;
using app.plataforma.Identity;
using app.plataforma.Interfaces;
using AspNetCore.Identity.Mongo.Mongo;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.IO.Compression;
using System.Text;

namespace app.plataforma.Services;

public class PostagensService : IPostagensService
{
    private readonly MongoDBContext _context;
    private UserManager<ApplicationUser> _userManager;
    private readonly string bucket = "BUCKET-ARQUIVOS";
    private readonly GridFSBucket fsBucket = null;
    private readonly IWebHostEnvironment _environment;
    public PostagensService(MongoDBContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
    {
        _context = context;
        _userManager = userManager;
        _environment = environment;
        fsBucket = new GridFSBucket(_context.Postagens.Database, new GridFSBucketOptions { BucketName = bucket });
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
        var nomeArquivo = Guid.NewGuid().ToString() + ".mp4";
        var objectId = await UploadFile(obj.arquivoBlob.ToString(), nomeArquivo);
        obj.arquivoId = objectId;
        //obj.arquivoUpload = nomeArquivo;
        obj.nomeArquivo = nomeArquivo;
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
        foreach (var postagem in lstPostagens)
        {
            postagem.arquivoUpload = await GetFile(postagem.arquivoId);
        }

        //var result = lstPostagens.Select(x => { x.arquivoUpload = Task.Run(async () => GetFile(x.arquivoId).Result).Result; return x; });
        return lstPostagens;
    }

    public byte[] Zip(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);

        using (var msi = new MemoryStream(bytes))
        using (var mso = new MemoryStream())
        {
            using (var gs = new GZipStream(mso, CompressionMode.Compress))
            {
                msi.CopyTo(gs);
                //CopyTo(msi, gs);
            }

            return mso.ToArray();
        }
    }

    public async Task<ObjectId> UploadFile(object blobFile, string savename = null)
    {

        byte[] textBytes = Encoding.UTF8.GetBytes(blobFile.ToString());

        string path = Path.Combine(_environment.WebRootPath, "upload", savename);

        File.WriteAllBytes(path, textBytes);




        if (_context.BucketFileInfo.Find(info => info.FileName.Equals(savename)).Any())
            throw new GridFSException($"\'{savename}\' arquivo já existente");


        //var id = await fsBucket.UploadFromBytesAsync(savename, Zip(file), new GridFSUploadOptions() { DisableMD5 = false, ChunkSizeBytes = 261120 });

        var id = await fsBucket.UploadFromBytesAsync(savename, textBytes, new GridFSUploadOptions() { });

        var fileInfo = new BucketFileInfo(DateTime.UtcNow)
        {
            Id = id,
            FileName = savename,
            FileSize = textBytes.Length,
        };

        _context.BucketFileInfo.InsertOne(fileInfo);

        return id;
    }


    public async Task<ObjectId> UploadFile(string file, string savename = null)
    {



        string path = Path.Combine(_environment.WebRootPath, "upload", savename);

        //File.WriteAllText(path, file.Replace("data:video/mp4;base64,", string.Empty));

        string base64data = file.Replace("data:video/mp4;base64,", string.Empty);
        base64data = base64data.Replace("data:video/mp4;ogg;base64,", string.Empty);
        byte[] ret = Convert.FromBase64String(base64data);

        await File.WriteAllBytesAsync(path, ret);


        if (_context.BucketFileInfo.Find(info => info.FileName.Equals(savename)).Any())
            throw new GridFSException($"\'{savename}\' arquivo já existente");


        //var id = await fsBucket.UploadFromBytesAsync(savename, Zip(file), new GridFSUploadOptions() { DisableMD5 = false, ChunkSizeBytes = 261120 });

        byte[] textBytes = Encoding.ASCII.GetBytes(file);
        var id = await fsBucket.UploadFromBytesAsync(savename, ret, new GridFSUploadOptions() { });

        var fileInfo = new BucketFileInfo(DateTime.UtcNow)
        {
            Id = id,
            FileName = savename,
            FileSize = textBytes.Length,
        };

        _context.BucketFileInfo.InsertOne(fileInfo);

        return id;
    }

    public async Task<string> GetFile(ObjectId? id)
    {
        var bucketFileInfo = _context.BucketFileInfo.Find(fileInfo => fileInfo.Id.Equals(id));
        var result = string.Empty;

        if (bucketFileInfo.Any())
            result = System.Text.Encoding.Default.GetString(fsBucket.DownloadAsBytes(id));
        return result;
    }



}