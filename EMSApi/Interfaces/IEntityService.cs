using EMSApi.Models;

namespace EMSApi.Services
{
    public interface IEntityService<T> where T :BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity, string performedBy);
        Task UpdateAsync(T entity, string performedBy);
        Task DeleteAsync(int id, string performedBy);
    }
}
