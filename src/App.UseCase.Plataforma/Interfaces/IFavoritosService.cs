namespace app.plataforma.Interfaces;

public interface IFavoritosService
{
    Task<List<Favoritos>> ObterListaPorIdUsuarioAsync(object IdUsuario);
    Task InserirAsync(Favoritos obj);
    Task DeletarAsync(object Id);
    Task DeletarTodosAsync(object IdUsuario);
}
