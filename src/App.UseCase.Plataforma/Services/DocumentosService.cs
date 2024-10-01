using app.plataforma;
using app.plataforma.Interfaces;
using app.plataforma.Models;
using MongoDB.Driver;

namespace App.UseCase.Plataforma.Services;

public class DocumentosService : IDocumentosService
{
    private readonly MongoDBContext _context;
    public DocumentosService(MongoDBContext context)
    {
        _context = context;
    }

    public async void DeleteAsync(object Id)
    {

        var documento = await _context.Documentos.Find(p => p.Id == Id).FirstOrDefaultAsync();

        if (documento != null)        
          await _context.Documentos.DeleteOneAsync(p => p.Id == Id);
    }

    public async Task<List<Documentos>> GetAllAsync()
    { 
        return await _context.Documentos.Find(_ => true).ToListAsync();
    }

    public async void InsertAsync(Documentos obj)
    {
        await _context.Documentos.InsertOneAsync(obj);
    }

    public async void UpdateAsync(Documentos obj)
    {
        var documento = await _context.Documentos.Find(p => p.Id == obj.Id).FirstOrDefaultAsync();

        if (documento != null)
          await _context.Documentos.ReplaceOneAsync(p => p.Id == obj.Id, obj);
    }
}