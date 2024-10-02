using app.plataforma.Handlers;
using app.plataforma.Interfaces;
using app.plataforma.Models;

namespace app.plataforma.Services
{
    public interface IPostagensService : IServiceBase<Postagens>
    {
        Task DeletarTodosPorIdUsuarioAsync(object IdUsuario);

        Task<IUsuarioPostagem> CarregarDadosPorIdUsuarioAsync(object IdUsuario);
    }
}
