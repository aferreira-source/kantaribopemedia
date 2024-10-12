using app.plataforma.Handlers;
using MongoDB.Bson;

namespace app.plataforma.Interfaces;

public interface IPostagensService : IServiceBase<Postagens>
{
    Task DeletarTodosPorIdUsuarioAsync(object IdUsuario);
    Task<IUsuarioPostagem> CarregarDadosPorIdUsuarioAsync(object IdUsuario);
    Task<IEnumerable<Postagens>> ObterPostagens();
    Task<ObjectId> UploadFile(string file, string savename = null);
    Task<string> GetFile(ObjectId? id);
}
