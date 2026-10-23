using System;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Common
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
