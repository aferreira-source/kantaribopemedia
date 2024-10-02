using app.plataforma;
using app.plataforma.Models;
using app.plataforma.Services;
using MongoDB.Driver;

namespace App.UseCase.Plataforma.Services;

public class FavoritosService : IFavoritosService
{
    private readonly MongoDBContext _context;
    public FavoritosService(MongoDBContext context)
    {
        _context = context;
    }
    public async Task DeletarAsync(object Id)
    {
        var favorito = await _context.Favoritos.Find(p => p.Id == Id).FirstOrDefaultAsync();

        if (favorito != null)
            await _context.Favoritos.DeleteOneAsync(p => p.Id == Id);
    }

    public async Task DeletarTodosAsync(object IdUsuario)
    {
        var filter = Builders<Favoritos>.Filter.Where(x => x.usuarioId == IdUsuario);
        await _context.Favoritos.DeleteManyAsync(filter);
    }
  
    public async Task InserirAsync(Favoritos obj)
    {
        await _context.Favoritos.InsertOneAsync(obj);
    }

    public async Task<List<Favoritos>> ObterListaPorIdUsuarioAsync(object IdUsuario)
    {
       return await _context.Favoritos.Find(p => p.Id == IdUsuario).ToListAsync();
    }
}