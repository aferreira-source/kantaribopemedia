using app.plataforma.Handlers;
using app.plataforma.Identity;
using app.plataforma.Interfaces;
using AspNetCore.Identity.Mongo.Mongo;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Text;

namespace app.plataforma.Services;

public class PostagensService : IPostagensService
{
    private readonly MongoDBContext _context;
    private UserManager<ApplicationUser> _userManager;
    private readonly string bucket = "BUCKET-ARQUIVOS";
    private readonly GridFSBucket fsBucket = null;
    public PostagensService(MongoDBContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
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
        var nomeArquivo = Guid.NewGuid().ToString();
        var objectId = await UploadFile(obj.arquivoUpload.ToString(), nomeArquivo);
        obj.arquivoId = objectId;
        obj.arquivoUpload = string.Empty;
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

        var result = lstPostagens.Select(x => { x.arquivoUpload = Task.Run(async () => GetFile(x.arquivoId).Result).Result; return x; });
        return result;
    }


    public async Task<ObjectId> UploadFile(string file, string savename = null)
    {
        var memory = new MemoryStream(Encoding.UTF8.GetBytes(file));



        //using (FileStream file = new(file, FileMode.Create, FileAccess.Write))
        //{
        //    // Convert text to bytes
        //    byte[] textBytes = Encoding.ASCII.GetBytes("sometext\nsomemoretext\n");
        //    file.Write(textBytes, 0, textBytes.Length);

        //    // Compress and write the buffer
        //    using (GZipStream zipStream = new(file, CompressionMode.Compress))
        //    {
        //        zipStream.Write(buffer, 0, buffer.Length);
        //    }
        //}


        //Byte[] bytes = Encoding.ASCII.GetBytes(file);

        if (_context.BucketFileInfo.Find(info => info.FileName.Equals(savename)).Any())
            throw new GridFSException($"\'{savename}\' arquivo já existente");


        var id = await fsBucket.UploadFromStreamAsync(savename, memory, new GridFSUploadOptions() { DisableMD5 = false, ChunkSizeBytes = 261120 });

        var fileInfo = new BucketFileInfo(DateTime.UtcNow)
        {
            Id = id,
            FileName = savename,
            FileSize = memory.Length,
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