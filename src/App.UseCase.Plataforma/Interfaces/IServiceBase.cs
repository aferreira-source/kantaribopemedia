using app.plataforma.Models;

namespace app.plataforma.Interfaces
{
    public interface IServiceBase<T> where T : class
    {
        Task<List<T>> GetAllAsync();        
        void InsertAsync(T obj);
        void UpdateAsync(T obj);
        void DeleteAsync(object Id);
    }
}
