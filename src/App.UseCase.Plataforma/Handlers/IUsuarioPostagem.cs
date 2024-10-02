using app.plataforma.Models;

namespace app.plataforma.Handlers
{
    public interface IUsuarioPostagem
    {
        //Postagens
        Task<List<Postagens>> ObterPostagemAsync();
        Task AdicionarPostagemAsync(Postagens obj);
        Task AlterarPostagemAsync(Postagens obj);
        Task DeletarPostagemAsync(object id);
        Task DeletarTodasPostagemPorIdUsuarioAsync();

        //Favoritos
        Task<List<Favoritos>> ObterFavoritosPorIdUsuarioAsync();
        Task AdicionarFavoritoAsync(Favoritos obj);
        Task DeletarFavoritoAsync(object id);
        Task DeletarTodosFavoritosPorIdUsuarioAsync();
        //Usuario
        //Task<UsuarioPostagem> CarregarDadosAsync(object IdUsuario);
    }
}
