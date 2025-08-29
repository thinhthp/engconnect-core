using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngConnect.Repositories.Common
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();

        // ISomethingRepository SomethingRepository { get; }
        // Right here baby
    }
}
