using app.plataforma.Models;

namespace app.plataforma.Services
{
    public interface IFavoritosService
    {
        Task<List<Favoritos>> ObterListaPorIdUsuarioAsync(object IdUsuario);
        Task InserirAsync(Favoritos obj);
        Task DeletarAsync(object Id);
        Task DeletarTodosAsync(object IdUsuario);
    }
}
