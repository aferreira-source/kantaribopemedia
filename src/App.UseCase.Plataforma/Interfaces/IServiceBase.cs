namespace app.plataforma.Interfaces
{
    public interface IServiceBase<T> where T : class
    {
        Task<List<T>> ObterTodosPorIdUsuarioAsync(object IdUsuario);        
        Task InserirAsync(T obj);
        Task AlterarAsync(T obj);
        Task DeletarAsync(object Id);
    }
}
