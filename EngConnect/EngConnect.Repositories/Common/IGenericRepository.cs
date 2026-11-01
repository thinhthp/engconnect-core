using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Common
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
