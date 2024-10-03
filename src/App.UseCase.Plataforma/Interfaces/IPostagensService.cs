using app.plataforma.Handlers;

namespace app.plataforma.Interfaces
{
    public interface IPostagensService : IServiceBase<Postagens>
    {
        Task DeletarTodosPorIdUsuarioAsync(object IdUsuario);

        Task<IUsuarioPostagem> CarregarDadosPorIdUsuarioAsync(object IdUsuario);
    }
}
