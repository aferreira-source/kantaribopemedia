using app.plataforma.Handlers;

namespace app.plataforma.Interfaces;

public interface IPostagensService : IServiceBase<Postagens>
{
    Task DeletarTodosPorIdUsuarioAsync(object IdUsuario);
    Task<IUsuarioPostagem> CarregarDadosPorIdUsuarioAsync(object IdUsuario);
    Task<IEnumerable<Postagens>> ObterPostagens();
    Task<BucketFileInfo> UploadFile(string file, string savename = null);
    //Task<string> GetFile(ObjectId? id);
}
