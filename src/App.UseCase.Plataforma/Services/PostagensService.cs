using app.plataforma;
using app.plataforma.Handlers;
using app.plataforma.Models;
using app.plataforma.Services;
using MongoDB.Driver;

namespace App.UseCase.Plataforma.Services;

public class PostagensService : IPostagensService
{
    private readonly MongoDBContext _context;
    public PostagensService(MongoDBContext context)
    {
        _context = context;
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
}